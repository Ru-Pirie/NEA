using BackendLib;
using BackendLib.Data;
using BackendLib.Interfaces;
using LocalApp.CLI;
using System;
using System.IO;

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
                    new[]
                    {
                        "Process New Image Into Map Data File", "Recall Map From Data File", "Exit Program", "Dev Test"
                    });

                switch (opt)
                {
                    // New
                    case 0:
                        RunNewImage(m, i, l);
                        break;
                    // Recall
                    case 1:
                        Map recalledMap = new Map();

                        break;
                    case 2:
                        running = false;
                        break;
                    case 3:
                        DevTest(ref m, ref i, ref l);
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
                        "Multi-threaded - Fast, all options decided at the start",
                        "Synchronous - Slow, options can be changed after each step and steps can be repeated"
                    });

                IHandler handler = opt == 0 ? new AsyncEdgeDetection(m, i, l, rawImage, runGuid) : (IHandler)new SyncEdgeDetection(m, i, l, rawImage, runGuid);
                handler.Start();
                double[,] resultOfEdgeDetection = handler.Result();

                // TODO Next section move onto graph stuff

                l.EndSuccessRun(runGuid);
            }
            catch (Exception ex)
            {
                l.EndErrorRun(runGuid, ex);
            }
        }

        private static void DevTest(ref Menu m, ref Input i, ref Log l)
        {
            int opt = i.GetOption("Dev Test Options",
                new[] { "Wipe logs and run files including all saves", "Run automated demo", "Resize window", "N/A" });

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
                // Resize window    
                case 2:
                    m = new Menu("Author: Rubens Pirie", "\x1b[38;5;119mDevelopment Mode REBOOT\x1b[0m");
                    i = new Input(m);
                    l = new Log(m);

                    m.Setup();

                    break;


                case 3:


                    break;
            }
        }
    }
}