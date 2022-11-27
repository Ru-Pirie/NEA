using BackendLib;
using BackendLib.Data;
using BackendLib.Processing;
using LocalApp.CLI;
using LocalApp.WindowsForms;
using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace LocalApp.Processes
{
    internal class RoadSequence
    {
        private readonly Menu _menuInstance;
        private readonly Log _logInstance;
        private readonly Guid _runGuid;
        private double[,] _cannyEdgeDetectionResult;
        private readonly MapFile _saveFile;
        private Structures.RoadResult _roadResult;

        public RoadSequence(Menu menuInstance, Log logInstance, Guid currentGuid, double[,] cannyResult, MapFile saveFile)
        {
            _menuInstance = menuInstance;
            _logInstance = logInstance;
            _runGuid = currentGuid;
            _cannyEdgeDetectionResult = cannyResult;
            _saveFile = saveFile;
            _roadResult = new Structures.RoadResult();
        }

        public Structures.RoadResult Result() => _roadResult;

        public void Start()
        {
            Input inputHandel = new Input(_menuInstance);

            InvertImage(inputHandel);

            DetectRoads(inputHandel);

            if (_saveFile != null)
            {
                _saveFile.PathImage = new Bitmap(_roadResult.PathBitmap);
                _saveFile.CombinedImage = Utility.CombineBitmap(_saveFile.OriginalImage, _roadResult.PathBitmap);
                string path = _saveFile.Save(_runGuid);

                string saveName = _runGuid.ToString();

                if (bool.Parse(Settings.UserSettings["shortNames"]
                        .Item1))
                {

                    saveName = _saveFile.Name.Replace(' ', '_');
                    File.Move(path,
                        path.Replace(Path.GetFileName(path)
                                .Split('.')[0],
                            saveName));
                }

                if (bool.Parse(Settings.UserSettings["zipOnComplete"]
                        .Item1))
                {
                    Directory.CreateDirectory("temp");
                    Directory.CreateDirectory("temp/images");

                    string[] files = Directory.GetFiles($"./runs/{_runGuid.ToString("N").ToUpper()}", "*.*", SearchOption.AllDirectories);
                    foreach (string newPath in files)
                    {
                        File.Copy(newPath, newPath.Replace($"./runs/{_runGuid.ToString("N").ToUpper()}", "temp/images"));
                    }

                    File.Copy($"./logs/{_runGuid}.txt", "temp/log.txt");
                    File.Copy($"./saves/{saveName}.vmap", "temp/map.vmap");
                    ZipFile.CreateFromDirectory("temp", $"RUN-{_runGuid}");
                }



            }
        }

        private void InvertImage(Input inputHandel)
        {
            bool invert = Utility.IsYes(inputHandel.GetInput("Invert image (y/n)?"));
            if (invert)
            {
                _cannyEdgeDetectionResult = Utility.InverseImage(_cannyEdgeDetectionResult);
                ViewImageForm invertImageForm = new ViewImageForm(_cannyEdgeDetectionResult.ToBitmap());
                invertImageForm.ShowDialog();
                if (_saveFile != null) _saveFile.IsInverted = true;
            }
            if (_saveFile != null) _saveFile.IsInverted = false;

            _menuInstance.WriteLine();
        }

        private void DetectRoads(Input inputHandel)
        {
            bool happy = true;

            double threshold = 0.3;

            while (happy)
            {
                if (inputHandel.TryGetDouble(
                        $"Value for Threshold (Default: {threshold}, Range: 0 <= x < 1)",
                        out double newThreshold) && newThreshold > 0 && newThreshold < 1 && newThreshold != threshold)
                {
                    _logInstance.Warn(_runGuid, $"Changed threshold {threshold} -> {newThreshold}");
                    _menuInstance.WriteLine($"{Log.Green}Changed: {threshold} -> {newThreshold}{Log.Blank}");
                    threshold = newThreshold;
                }
                else _menuInstance.WriteLine($"{Log.Orange}Kept Default: {threshold}{Log.Blank}");
                _menuInstance.WriteLine();

                RoadDetection roadDetector = new RoadDetection(_cannyEdgeDetectionResult, threshold);
                ProgressBar pb = new ProgressBar("Road Detection", _cannyEdgeDetectionResult.Length / 100 * 3, _menuInstance);

                pb.DisplayProgress();
                roadDetector.Start(pb.GetIncrementAction());

                _roadResult = roadDetector.Result();
                ViewImageForm roadForm = new ViewImageForm(_roadResult.PathBitmap);
                roadForm.ShowDialog();

                _menuInstance.ClearUserSection();

                if (Utility.IsYes(inputHandel.GetInput("Are you happy with this lower threshold you should see your roads, if you don't try decreasing the threshold if you see too much then increase the threshold. (y/n)?"))) happy = false;
            }
        }
    }
}
