using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendLib;
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
            string path = 

            Pre preProcess = new Pre();
        }

    }
}
