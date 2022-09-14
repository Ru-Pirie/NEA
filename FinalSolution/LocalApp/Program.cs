using System;
using System.Globalization;
using System.IO;
using BackendLib;
using BackendLib.Processing;
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
                        RunNewImage(m, i, l);

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

        private static void RunNewImage(Menu m, Input i, Log l)
        {
            Guid runGuid = Logger.CreateRun();
            l.Event(runGuid, $"Begin processing of new image (Run Id: {runGuid}).");

            NewImage newImage = new NewImage(m, i, l, runGuid);

            try
            {
                Structures.RawImage rawImage = newImage.Read();

                int opt = i.GetOption("Select a version of edge detection to run:",
                    new[]
                    {
                        "Multithreaded - Fast, all options decided at the start",
                        "Synchronous - Slow, options can be changed after each step and steps can be repeated"
                    });

                double[,] cannyImage;

                if (opt == 0) cannyImage = AsyncEdgeDetection();
                else cannyImage = SyncEdgeDetection();

                l.EndSuccessRun(runGuid);
            }
            catch (Exception ex)
            {
                l.EndErrorRun(runGuid, ex);
            }
        }

        private static double[,] AsyncEdgeDetection(Menu m, Input i, Log l)
        {
            return new double[0, 0];
        }

        private static double[,] SyncEdgeDetection(Menu m, Input i, Log l)
        {
            return new double[0, 0];
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
