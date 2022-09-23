using BackendLib;
using BackendLib.Data;
using BackendLib.Interfaces;
using LocalApp.CLI;
using System;
using System.IO;
using BackendLib.Processing;
using LocalApp.WindowsForms;

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
            menu.SetPage("Welcome");

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
                        m.WriteLine("Before we begin, at the start of this you will be asked to supply an image to be converted into a map. After you have done this the following will occur:");
                        m.WriteLine();
                        m.WriteLine("1. The image you supplied will be checked to see if it is suitable, if it is you will be shown said image and asked to confirm if you wish to proceed.");
                        m.WriteLine("2. The image will have edge detection performed on it to get its roads, you will have the opportunity to enter the variables which control the edge detection.");
                        m.WriteLine("3. The result of your map will either be inverted (black pixels to white or vice versa) depending on the result.");
                        m.WriteLine("4. This image will then have a combination of holistic algorithms run on it to pick out roads.");
                        m.WriteLine("5. You can pathfind through your processed map image.");
                        m.WriteLine("6. You can chose to save that map or not.");
                        m.WriteLine("7. That's it!");
                        m.WriteLine();
                        i.GetInput("\x1b[38;5;208m(Enter to continue)\x1b[0m");

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

                // Show Before ask for confirmation after?
                ViewImageForm beforeForm = new ViewImageForm(rawImage.Pixels.ToBitmap());
                m.WriteLine("Click and Press Enter");
                beforeForm.ShowDialog();
                // Confirm correct image here before progressing?

                int opt = i.GetOption("Select a version of edge detection to run:",new[] {
                        "Multi-threaded - Fast, all options decided at the start",
                        "Synchronous - Slow, options can be changed after each step and steps can be repeated" });
                
                IHandler handler = opt == 0 ? new AsyncEdgeDetection(m, i, l, rawImage, runGuid) : (IHandler)new SyncEdgeDetection(m, i, l, rawImage, runGuid);
                handler.Start();  
                double[,] resultOfEdgeDetection = handler.Result();

                //Show After to User
                ViewImageForm edgeImageForm = new ViewImageForm(resultOfEdgeDetection.ToBitmap());
                m.WriteLine("Click and Press Enter");
                edgeImageForm.ShowDialog();

                m.ClearUserSection();
                m.WriteLine("In order for the road detection to function properly there must be a single white line joining roads together, not two either side. If there where two either side then the image must be inverted.");
                m.WriteLine();
                
                
                bool invert = i.GetInput("Invert image (y/n)? ").ToLower() == "y";
                if (invert)
                {
                    resultOfEdgeDetection = Utility.InverseImage(resultOfEdgeDetection);


                    ViewImageForm invertImageForm = new ViewImageForm(resultOfEdgeDetection.ToBitmap());
                    m.WriteLine("Click and Press Enter");
                    invertImageForm.ShowDialog();
                }
                


                // TODO prompt to move onto road detection add user input for threshold
                RoadDetection roadDetector = new RoadDetection(resultOfEdgeDetection, 0.3);
                


                // TODO Next section move road detection then graph stuff

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
                new[] { "Wipe logs and run files including all saves", "Run automated demo", "Resize window", "TEst Special Stuff" });

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
                    m.WriteLine($"Width - User: {Console.WindowWidth * 3 / 4 * 8}");
                    m.WriteLine($"Width - Total: {Console.WindowWidth * 8}");
                    m.WriteLine($"Height - User:{Console.WindowHeight * 5 / 6 * 16}");
                    m.WriteLine($"Height - Total:{Console.WindowHeight * 16}");

                    i.GetInput("");

                    break;
            }
        }
    }
}