using System;
using System.Net.Sockets;
using System.Threading;
using FinalSolution.src.local
    ;
using FinalSolution.src.utility;

namespace FinalSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Rubens Pirie Compsci NEA | Boot";
            int opt = Menu.GetOption("Please select the version you would like to use:", new string[] { "Local version", "Web version" });
            switch (opt)
            {
                case 0:
                    Console.Title = "Rubens Pirie Compsci NEA | Local";
                    StartLocal();
                    break;
                case 1:
                    Console.Title = "Rubens Pirie Compsci NEA | Cloud";
                    StartBlazor();
                    break;
            }

            Console.ReadLine();
        }

        private static void StartLocal()
        {
            new LocalApplication();
        }

        private static void StartBlazor()
        {
            throw new NotImplementedException("This is an extention objective and has not been completed yet.");
        }
    }
}
