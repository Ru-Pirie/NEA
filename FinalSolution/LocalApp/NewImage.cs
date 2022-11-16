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

            string path = inputHandel.GetInput("Please enter the path of the image you wish to process into a map:");
            _logInstance.Event(_runGuid, $"Looking for image at {path}");

            Pre preProcess = new Pre(path);

            ProgressBar progressBar = new ProgressBar("Pre Processing Image", 4, _menuInstance);
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

            bool saveAsBinary = Utility.IsYes(inputHandel.GetInput("Would you like to save this map in a custom file to be reused later (y/n)?"));
            MapFile mapSave = saveAsBinary ? new MapFile() : null;

            if (saveAsBinary) mapSave.Type = inputHandel.GetOption("What type of image are you supplying:", new[] { "Screenshot", "Hand Drawn", "Photograph", "Other" });
            if (saveAsBinary) mapSave.Name = inputHandel.GetInput("Enter a name for image:");
            _menuInstance.WriteLine();
            if (saveAsBinary) mapSave.Description = inputHandel.GetInput("Enter a brief description about this image:");

            Structures.RawImage result = preProcess.Result();
            if (saveAsBinary) result.MapFile = mapSave;
            else result.MapFile = null;
            if (saveAsBinary) mapSave.OriginalImage = result.Pixels.ToBitmap();

            return result;
        }
    }
}
