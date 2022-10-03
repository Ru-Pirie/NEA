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
        private readonly Menu _m;
        private readonly Input _i;
        private readonly Log _l;

        public NewImage(Menu m, Input i, Log l, Guid runGuid)
        {
            _runGuid = runGuid;
            _m = m;
            _i = i;
            _l = l;
        }

        public Structures.RawImage Read()
        {
            string path = _i.GetInput("Please enter the path of the image you wish to process into a map:");
            _l.Event(_runGuid, $"Looking for image at {path}");

            Pre preProcess = new Pre(path);

            ProgressBar progressBar = new ProgressBar("Pre Processing Image", 4, _m);
            progressBar.DisplayProgress();

            try
            {
                preProcess.Start(progressBar.GetIncrementAction());
                _l.Event(_runGuid, "Completed pre processing of image.");
            }
            catch (PreprocessingException ex)
            {
                _l.Error(_runGuid, ex.Message);
                throw new Exception("An expected occurred while pre processing your image.", ex);
            }
            catch (Exception ex)
            {
                _l.Error(ex.Message);
                throw new Exception("An unexpected occurred while pre processing your image.", ex);
            }

            _m.ClearUserSection();

            bool saveAsBinary = Utility.IsYes(_i.GetInput("Would you like to save this map in a custom file to be reused later (y/n)?"));
            Map mapSave = saveAsBinary ? new Map() : null;

            if (saveAsBinary) mapSave.Type = _i.GetOption("What type of image are you supplying:", new[] { "Screenshot", "Hand Drawn", "Photograph", "Other" });
            if (saveAsBinary) mapSave.Description = _i.GetInput("Enter a brief description about this image:");

            Structures.RawImage result = preProcess.Result();
            if (saveAsBinary) result.MapFile = mapSave;
            if (saveAsBinary) mapSave.OriginalImage = result.Pixels.ToBitmap();

            return result;
        }
    }
}
