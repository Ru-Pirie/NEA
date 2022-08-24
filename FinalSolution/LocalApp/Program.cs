using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackendLib;
using BackendLib.Data;
using LocalApp.CLI;

namespace LocalApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Menu menu = new Menu("Author: Rubens Pirie", "\x1b[38;5;119mDevelopment Mode\x1b[0m");
            Input inputs = new Input(menu);
            Log logger = new Log(menu);

            menu.Setup();
            logger.Event("Program has started and menu has been created successfully.");

            Map testMap = new Map(@"P:\NEA\FinalSolution\LocalApp\bin\Debug\saves\0c7ee29e-66d1-44cb-9572-9a06fc455641.vmap");
            testMap.Initialize();

            testMap.OriginalImage.Save("a.png");
            testMap.PathImage.Save("b.png");
            testMap.CombinedImage.Save("c.png");

            //Map testMap = new Map();
            //testMap.Name = "Test";
            //testMap.Description = "This is a test description which hopefully works";
            //testMap.Type = "development";
            //testMap.TimeCreated = DateTimeOffset.Now;
            //testMap.OriginalImage = new Bitmap(@"P:\NEA\writeup\images\edgeDetectionExperiment\a.jpg");
            //testMap.PathImage = new Bitmap(@"P:\NEA\writeup\images\edgeDetectionExperiment\f.jpg");
            //testMap.CombinedImage = new Bitmap(@"P:\NEA\writeup\images\edgeDetectionExperiment\k.jpg");

            //testMap.Save(Logger.CreateRun());

            Run(menu, inputs, logger);
        }

        private static void Run(Menu m, Input i, Log l)
        {
            bool running = true;

            while (running)
            {
                int opt = i.GetOption("Please select an option to continue:",
                    new[] { "Process New Image Into Map Data File", "Recall Map From Data File" });

                switch (opt)
                {
                    case 0: 
                        m.SetPage(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                        break;
                    case 1:
                        break;
                }
            }
        }

    }
}
