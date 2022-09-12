using System;
using System.Globalization;
using System.IO;
using BackendLib;
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

            Run(menu, inputs, logger);
        }

        private static void Run(Menu m, Input i, Log l)
        {
            bool running = true;

            while (running)
            {
                int opt = i.GetOption("Please select an option to continue:",
                    new[] { "Process New Image Into Map Data File", "Recall Map From Data File", "Exit Program", "Dev Test" });

                switch (opt)
                {
                    // New
                    case 0:
                        Guid newRun = Logger.CreateRun();
                        l.Event(newRun, $"Begin processing of new image (Run Id: {newRun}).");




                        break;
                    // Recall
                    case 1:



                        break;
                    case 2:
                        running = false;
                        break;
                    case 3:
                        DevTest(m, i, l);
                        break;
                }
            }
        }

        private static void DevTest(Menu m, Input i, Log l)
        {
            int opt = i.GetOption("Dev Test Options",
                new[] { "Wipe logs and run files including all saves", "Run automated demo", "N/A", "N/A" });

            switch (opt)
            {
                // wipe logs
                case 0:
                    Directory.Delete("./logs", true);
                    Directory.Delete("./runs", true);
                    Directory.Delete("./saves", true);
                    _ = new Logger(true);
                    
                    break;


                // run auto demo
                case 1:



                    break;
                case 2:




                    break;




                case 3:




                    break;
            }
        }

    }
}
