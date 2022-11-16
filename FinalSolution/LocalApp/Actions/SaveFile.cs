using BackendLib.Data;
using BackendLib.Exceptions;
using LocalApp.CLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalApp
{
    public class SaveFile
    {
        private readonly Guid _runGuid;
        private readonly Menu _menuInstance;
        private readonly Log _logInstance;

        public SaveFile(Menu menu, Log logger, Guid runGuid)
        {
            _runGuid = runGuid;
            _menuInstance = menu;
            _logInstance = logger;
        }

        public MapFile Read()
        {
            Input inputHandel = new Input(_menuInstance);

            string path = inputHandel.GetInput("Please enter the path of the map which you wish to recall:");
            _logInstance.Event(_runGuid, $"Looking for map file at {path}");

            ProgressBar progressBar = new ProgressBar("Recalling Saved Map File", 10, _menuInstance);
            progressBar.DisplayProgress();

            MapFile result = new MapFile(path);

            try
            {
                result.Initialize(progressBar.GetIncrementAction());
                _logInstance.Event(_runGuid, "Completed recollection.");
            }
            catch (MapFileException ex)
            {
                _logInstance.Error(_runGuid, ex.Message);
                throw new Exception("An expected occurred while recalling your save file.", ex);
            }
            catch (Exception ex)
            {
                _logInstance.Error(ex.Message);
                throw new Exception("An unexpected occurred while recalling your save file.", ex);
            }


            return result;
        }

    }
}
//a 