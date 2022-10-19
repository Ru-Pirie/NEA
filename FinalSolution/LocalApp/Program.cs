using BackendLib;
using BackendLib.Data;
using BackendLib.Interfaces;
using LocalApp.CLI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using BackendLib.Datatypes;
using BackendLib.Processing;
using LocalApp.WindowsForms;
using Microsoft.Data.Sqlite;

/**
 * A* search tends to work well in a 
 * Binary Search in priority queue
 * 
 * 
 * 
 * 
 * **/

/**
 * A* search tends to work well in a 
 * Binary Search in priority queue
 * 
 * 
 * 
 * 
 * **/

namespace LocalApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Menu menu = new Menu("Author: Rubens Pirie", $"{Log.Green}Development Mode{Log.Blank}");
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
                        i.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");
                        m.WriteLine();

                        RunNewImage(m, i, l);
                        break;
                    // Recall
                    case 1:
<<<<<<< HEAD
                        string path = i.GetInput("Please enter a file to recall!?");

                        Map recalledMap = new Map(path);
                        recalledMap.Initialize();

                        double[,] doubles = recalledMap.PathImage.ToDoubles(Utility.GetIfExists);
                        Graph<Structures.Cord> myGraph = doubles.ToGraph();
                        Traversal<Structures.Cord> traversal = new Traversal<Structures.Cord>(myGraph);
=======
                        string path = i.GetInput("gib path");

                        Map recalledMap = new Map(path);
                        recalledMap.Initialize();
                        //ViewImageForm edgeImageForm = new ViewImageForm(recalledMap.CombinedImage);
                        //edgeImageForm.Show();
                        recalledMap.OriginalImage.Save("orig.png");
                        recalledMap.PathImage.Save("path.png");
                        recalledMap.CombinedImage.Save("comb.png");
>>>>>>> 252f99ed7118f2e9d8244c636a598d3cf409b3cd

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
            m.ClearUserSection();

            l.Event(runGuid, $"Begin processing of new image (Run Id: {runGuid}).");

            NewImage newImage = new NewImage(m, i, l, runGuid);

            try
            {
                Structures.RawImage rawImage = newImage.Read();

                // Show Before ask for confirmation after?
                ViewImageForm beforeForm = new ViewImageForm(rawImage.Pixels.ToBitmap());
                beforeForm.ShowDialog();
                m.ClearUserSection();

                m.WriteLine("Parsed file information:");
                m.WriteLine($"    Name: {Log.Green}{Path.GetFileNameWithoutExtension(rawImage.Path)}{Log.Blank}");
                m.WriteLine($"    Folder: {Log.Green}{Path.GetDirectoryName(rawImage.Path)}{Log.Blank}");
                m.WriteLine($"    File extension: {Log.Green}{Path.GetExtension(rawImage.Path)}{Log.Blank}");
                m.WriteLine();
                i.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");

                // Confirm correct image here before progressing?

                int opt = i.GetOption("Select a version of edge detection to run:", new[] {
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
                m.WriteLine("In order for the road detection to function properly there must be a a box encapsulating the road. It should look like an outline of the road, if there isn't one then select invert at the next prompt.");
                m.WriteLine();


                Map saveMapFile = rawImage.MapFile;

                bool invert = Utility.IsYes(i.GetInput("Invert image (y/n)?"));
                if (invert)
                {
                    resultOfEdgeDetection = Utility.InverseImage(resultOfEdgeDetection);
                    ViewImageForm invertImageForm = new ViewImageForm(resultOfEdgeDetection.ToBitmap());
                    invertImageForm.ShowDialog();
                    if (saveMapFile != null) saveMapFile.IsInverted = true;
                }
                if (saveMapFile != null) saveMapFile.IsInverted = false;



                // TODO prompt to move onto road detection add user input for threshold
                RoadDetection roadDetector = new RoadDetection(resultOfEdgeDetection, 0.3);
                ProgressBar pb = new ProgressBar("Road Detection", resultOfEdgeDetection.Length / 100 * 3, m);
                pb.DisplayProgress();
                roadDetector.Start(pb.GetIncrementAction());
                ViewImageForm roadForm = new ViewImageForm(roadDetector.Result().PathBitmap);
                roadForm.ShowDialog();

                if (saveMapFile != null)
                {
                    saveMapFile.PathImage = new Bitmap(roadDetector.Result().PathBitmap);
                    saveMapFile.CombinedImage = Utility.CombineBitmap(saveMapFile.OriginalImage, roadDetector.Result().PathBitmap);
                    saveMapFile.Save(runGuid);
                }



                // TODO Next section move road detection then graph stuff
                // TODO CHECK FOR REFERENCE TYPE BITMAP ISSUES
                // TEMP
                Graph<Structures.Cord> myGraph = roadDetector.Result().PathDoubles.ToGraph();
                Traversal<Structures.Cord> myTraversal = new Traversal<Structures.Cord>(myGraph);

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
                new[] { "Wipe logs and run files including all saves", "Run automated demo", "Resize window", "Test Special Stuff" });

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
                    PriorityQueue<string> priorityQueue = new PriorityQueue<string>();
                    priorityQueue.Enqueue("a", 5);
                    priorityQueue.Enqueue("b", 10);
                    priorityQueue.Enqueue("c", 1);
                    priorityQueue.Enqueue("d", 3);

                    for (int ad = 0; ad < 2; ad++)
                    {
                        Console.WriteLine(priorityQueue.Size);
                        Console.WriteLine(priorityQueue.Dequeue());
                    }

                    priorityQueue.Enqueue("bob", 2);
                    priorityQueue.Enqueue("super bob", 100);

                    for (int ad = 0; ad < 4; ad++)
                    {
                        Console.WriteLine(priorityQueue.Size);
                        Console.WriteLine(priorityQueue.Dequeue());
                    }


                    i.WaitInput("");

                    break;
                // Resize window    
                case 2:
                    m = new Menu("Author: Rubens Pirie", $"{Log.Green}Development Mode REBOOT{Log.Blank}");
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