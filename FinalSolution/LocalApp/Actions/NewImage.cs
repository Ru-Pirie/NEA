using BackendLib;
using BackendLib.Data;
using BackendLib.Exceptions;
using BackendLib.Processing;
using LocalApp.CLI;
using System;

namespace LocalApp
{
    internal class NewImage
    {
        private readonly Guid _runGuid;
        private readonly Menu _menuInstance;
        private readonly Log _logInstance;

        public NewImage(Menu menu, Log logger, Guid runGuid)
        {
            _runGuid = runGuid;
            _menuInstance = menu;
            _logInstance = logger;
        }

        public Structures.RawImage Read()
        {
            Input inputHandel = new Input(_menuInstance);

            string path =
                inputHandel.GetInput(
                    "Please enter the path of the image you wish to process into a map (you can click and drag an image from your file explorer here too):");
            _logInstance.Event(_runGuid, $"Looking for image at {path}");

            Pre preProcess = new Pre(path);

            ProgressBar progressBar = new ProgressBar("Pre-processing your image", 4, _menuInstance);
            progressBar.DisplayProgress();

            try
            {
                preProcess.Start(progressBar.GetIncrementAction());
                _logInstance.Event(_runGuid, "Completed pre processing of image.");
            }
            catch (PreprocessingException ex)
            {
                _logInstance.Error(_runGuid, ex.Message);
                throw new Exception("An expected occurred while pre processing your image.", ex);
            }
            catch (Exception ex)
            {
                _logInstance.Error(ex.Message);
                throw new Exception("An unexpected occurred while pre processing your image.", ex);
            }

            _menuInstance.ClearUserSection();

            bool saveAsBinary =
                Utility.IsYes(
                    inputHandel.TryGetInput(
                        "Would you like to save this map afterwards in a file to be reused later (y/n)?"));
            MapFile mapSave = saveAsBinary ? new MapFile() : null;

            if (saveAsBinary)
            {
                mapSave.Type = inputHandel.GetOption("What type of image are you supplying:",
                    new[] { "Screenshot", "Hand Drawn", "Photograph", "Other" });

                mapSave.Name = inputHandel.TryGetInput("Enter a name for image, or leave blank for 'None':");
                _menuInstance.WriteLine();

                mapSave.Description = inputHandel.TryGetInput("Enter a brief description about this image, or leave blank for 'None':");
            }

            Structures.RawImage result = preProcess.Result();
            if (saveAsBinary) result.MapFile = mapSave;
            else result.MapFile = null;
            if (saveAsBinary) mapSave.OriginalImage = result.Pixels.ToBitmap();

            return result;
        }
    }
}
