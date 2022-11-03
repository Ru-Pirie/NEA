using BackendLib;
using BackendLib.Data;
using BackendLib.Datatypes;
using BackendLib.Interfaces;
using BackendLib.Processing;
using LocalApp.CLI;
using LocalApp.WindowsForms;
using System;
using System.Drawing;
using System.IO;

namespace LocalApp
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Menu menu = new Menu("Author: Rubens Pirie", $"{Log.Green}Development Mode{Log.Blank}");
            Log logger = new Log(menu);

            menu.Setup();
            logger.Event("Program has started and menu has been created successfully.");
            menu.SetPage("Welcome");

            Run(menu, logger);
        }

        private static void Run(Menu menuInstance, Log CLILoggingInstance)
        {
            Input inputHandel = new Input(menuInstance);

            bool running = true;

            while (running)
            {
                int opt = inputHandel.GetOption("Please select an option to continue:",
                    new[]
                    {
                        "Process New Image Into Map Data File", "Recall Map From Data File", "Settings", "Exit Program", "??????????? ????ᯅ"
                    });

                switch (opt)
                {
                    // New
                    case 0:
                        TextWall.Welcome(menuInstance);
                        inputHandel.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");
                        menuInstance.WriteLine();

                        RunNewImage(menuInstance, CLILoggingInstance);
                        break;
                    // Recall
                    case 1:
                        string path = inputHandel.GetInput("Please enter the file path of the save to recall:");

                        // TODO ADD SOME OPTIONS HERE ID WHAT MAKE THEM UP


                        Map recalledMap = new Map(path);
                        recalledMap.Initialize();

                        double[,] doubles = recalledMap.PathImage.ToDoubles(Utility.GetIfExists);

                        Graph<Structures.Coord> myGraph = doubles.ToGraph();
                        Traversal<Structures.Coord> traversal = new Traversal<Structures.Coord>(myGraph);

                        PathfindImageForm myForm = new PathfindImageForm(recalledMap.OriginalImage, traversal, myGraph);
                        myForm.ShowDialog();
                        break;
                    case 2:
                        Settings settingsInstance = new Settings(menuInstance, CLILoggingInstance);
                        settingsInstance.Read();
                        settingsInstance.Update(settingsInstance.UserSettings, settingsInstance.UserSettings);
                        menuInstance.ClearUserSection();
                        break;
                    case 3:
                        running = false;
                        break;
                    case 4:
                        DevTest(ref menuInstance, ref inputHandel, ref CLILoggingInstance);
                        break;
                }
            }
        }

        private static void RunNewImage(Menu menu, Log logger)
        {
            Input i = new Input(menu);

            Guid runGuid = Logger.CreateRun();
            menu.ClearUserSection();

            logger.Event(runGuid, $"Begin processing of new image (Run Id: {runGuid}).");

            NewImage newImage = new NewImage(menu, logger, runGuid);

            try
            {
                Structures.RawImage rawImage = newImage.Read();

                // Show Before ask for confirmation after?
                ViewImageForm beforeForm = new ViewImageForm(rawImage.Pixels.ToBitmap());
                beforeForm.ShowDialog();
                menu.ClearUserSection();

                menu.WriteLine("Parsed file information:");
                menu.WriteLine($"    Name: {Log.Green}{Path.GetFileNameWithoutExtension(rawImage.Path)}{Log.Blank}");
                menu.WriteLine($"    Folder: {Log.Green}{Path.GetDirectoryName(rawImage.Path)}{Log.Blank}");
                menu.WriteLine($"    File extension: {Log.Green}{Path.GetExtension(rawImage.Path)}{Log.Blank}");
                menu.WriteLine();
                i.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");

                // Confirm correct image here before progressing?

                int opt = i.GetOption("Select a version of edge detection to run:", new[] {
                        "Multi-threaded - Fast, all options decided at the start",
                        "Synchronous - Slow, options can be changed after each step and steps can be repeated" });

                IHandler handler = opt == 0 ? new AsyncEdgeDetection(menu, logger, rawImage, runGuid) : (IHandler)new SyncEdgeDetection(menu, logger, rawImage, runGuid);
                handler.Start();
                double[,] resultOfEdgeDetection = handler.Result();

                //Show After to User
                ViewImageForm edgeImageForm = new ViewImageForm(resultOfEdgeDetection.ToBitmap());
                menu.WriteLine("Click and Press Enter");
                edgeImageForm.ShowDialog();

                menu.ClearUserSection();
                menu.WriteLine("In order for the road detection to function properly there must be a a box encapsulating the road. It should look like an outline of the road, if there isn't one then select invert at the next prompt.");
                menu.WriteLine();


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
                ProgressBar pb = new ProgressBar("Road Detection", resultOfEdgeDetection.Length / 100 * 3, menu);
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
                Graph<Structures.Coord> myGraph = roadDetector.Result().PathDoubles.ToGraph();
                Traversal<Structures.Coord> myTraversal = new Traversal<Structures.Coord>(myGraph);

                PathfindImageForm myForm = new PathfindImageForm(rawImage.Original, myTraversal, myGraph);
                myForm.ShowDialog();

                logger.EndSuccessRun(runGuid);
            }
            catch (Exception ex)
            {
                logger.EndErrorRun(runGuid, ex);
            }
        }





        // The unloved child of my code to be removed at some point
        private static void DevTest(ref Menu m, ref Input i, ref Log l)
        {
            int opt = i.GetOption("Dev Test Options",
                new[] { "Wipe logs and run files including all saves", "Min Queue Test", "Resize window", "Test Dijkstra" });

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
                    MinPriorityQueue<string> priorityQueue = new MinPriorityQueue<string>();
                    priorityQueue.Enqueue("a", 5);
                    priorityQueue.Enqueue("b", 10);
                    priorityQueue.Enqueue("c", 1);
                    priorityQueue.Enqueue("d", 3);

                    for (int ad = 0; ad < 2; ad++)
                    {
                        Console.WriteLine(priorityQueue.Dequeue());
                    }

                    priorityQueue.Enqueue("bob", 2);
                    priorityQueue.Enqueue("super bob", 100);
                    priorityQueue.ChangePriority("b", 200);

                    for (int ad = 0; ad < 4; ad++)
                    {
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
                    Bitmap testImage = new Bitmap("test.png");
                    Graph<Structures.Coord> testGraph = testImage.ToDoubles(Utility.GetIfExists).ToGraph();

                    Traversal<Structures.Coord> testTraversal = new Traversal<Structures.Coord>(testGraph);

                    PathfindImageForm myForm = new PathfindImageForm(testImage, testTraversal, testGraph);
                    myForm.ShowDialog();


                    Structures.Coord start = new Structures.Coord { X = 0, Y = 0 };
                    Structures.Coord goal = new Structures.Coord { X = 150, Y = 105 };

                    Structures.Coord[] res = Utility.RebuildPath(testTraversal.Dijkstra(start, (_) => 1), goal);


                    Bitmap testOut = new Bitmap(testImage);
                    foreach (var node in res)
                    {
                        testOut.SetPixel(node.X, node.Y, Color.Blue);
                    }

                    testOut.Save("testOut.png");

                    break;
            }
        }
    }
}