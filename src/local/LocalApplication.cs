using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalSolution.src.utility;

namespace FinalSolution.src.local
{
    public class LocalApplication
    {
        private bool running = true;

        public LocalApplication()
        {
            while (running)
            {
                int opt = Menu.GetOption("", new string[] { "", "" });
            }
        }

    }
}
