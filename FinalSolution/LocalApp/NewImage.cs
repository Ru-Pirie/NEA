using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendLib;
using BackendLib.Exceptions;
using BackendLib.Processing;
using LocalApp.CLI;

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
        }

        public Structures.RawImage Read()
        {
            string path = _i.GetInput("Please enter the path of the image you wish to process into a map:");

            Pre preProcess = new Pre(path);

            try
            {
                preProcess.Start();
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

            return preProcess.Result();
        }
    }
}
