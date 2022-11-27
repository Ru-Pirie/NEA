using BackendLib;
using BackendLib.Data;
using BackendLib.Datatypes;
using BackendLib.Interfaces;
using BackendLib.Processing;
using LocalApp.CLI;
using LocalApp.WindowsForms;
using System;
using System.Drawing;

namespace LocalApp
{
    public class Program
    {
        private static void Main()
        {
            Menu menu = new Menu("Author: Rubens Pirie", $"{Log.Green}Development Mode{Log.Blank}");
            Log logger = new Log(menu);

            Settings settings = new Settings(menu, logger);
            settings.Read();

            menu.Setup();
            logger.Event("Program has started and menu has been created successfully.");
            menu.SetPage("Welcome");

            Run(menu, logger, settings);
        }

        private static void Run(Menu menuInstance, Log CLILoggingInstance, Settings settingsInstance)
        {
            Input inputHandel = new Input(menuInstance);

            bool running = true;

            while (running)
            {
                int opt = inputHandel.GetOption("Please select an option to continue:",
                    new[]
                    {
                        "Process New Image Into Map Data File", "Recall Map From Data File", "Settings", "Exit Programᯅ"
                    });

                switch (opt)
                {
                    // New
                    case 0:
                        TextWall.ImageWelcome(menuInstance);
                        inputHandel.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");
                        menuInstance.WriteLine();

                        RunNewImage(menuInstance, CLILoggingInstance);
                        break;
                    // Recall
                    case 1:
                        TextWall.SaveWelcome(menuInstance);
                        inputHandel.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");
                        menuInstance.WriteLine();

                        RunSaveFile(menuInstance, CLILoggingInstance);
                        break;
                    // Settings
                    case 2:
                        SettingsControl settingsControl = new SettingsControl(settingsInstance, menuInstance, CLILoggingInstance);
                        settingsControl.Start();

                        menuInstance.ClearUserSection();
                        break;
                    // Exit
                    case 3:
                        running = false;
                        break;
                }
            }
        }

        private static double GetDistanceBetweenNodes(Structures.Coord start, Structures.Coord goal) => Utility.GetDistanceBetweenNodes(start, goal);

        private static void RunSaveFile(Menu menu, Log logger)
        {
            Input inputHandel = new Input(menu);
            Guid runGuid = Logger.CreateRun();

            menu.ClearUserSection();
            logger.Event(runGuid, $"Beginning recall of map file (Run Id: {runGuid})");

            SaveFile saveFile = new SaveFile(menu, logger, runGuid);

            try
            {
                MapFile recalledMap = saveFile.Read();

                //TODO ADD SOME OPTIONS HERE ID WHAT MAKE THEM UP SEE PROMPT


                // use for settings testing
                double[,] doubles = recalledMap.PathImage.ToDoubles(Utility.GetIfExists);

                Graph<Structures.Coord> myGraph = doubles.ToGraph();
                Traversal<Structures.Coord> traversal = new Traversal<Structures.Coord>(myGraph);

                Bitmap toSave = new Bitmap(recalledMap.OriginalImage);

                //var start = new Structures.Coord{ X = 122, Y = 1 };
                //var end = new Structures.Coord{X=360, Y=509};
                //var visitedOrder = traversal.ModifiedAStar(start, end, GetDistanceBetweenNodes);

                //for (int i = 0; i < visitedOrder.Count; i++)
                //{
                //    if (i % 1 == 0)
                //    {
                //        Logger.SaveBitmap(runGuid, toSave, $"aStarTest{i}");
                //        toSave.SetPixel(visitedOrder[i].X, visitedOrder[i].Y, Color.Purple);
                //    }
                //}


                PathfindImageForm myForm = new PathfindImageForm(recalledMap.OriginalImage, traversal, myGraph);
                myForm.ShowDialog();

                logger.EndSuccessSave(runGuid);
            }
            catch (Exception ex)
            {
                menu.ClearUserSection();
                menu.Error(ex.InnerException.Message);
                new Input(menu).WaitInput("");
                logger.EndError(runGuid, ex);
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

                TextWall.FileDetails(menu, rawImage);
                i.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");

                // Confirm correct image here before progressing?

                int opt = i.GetOption("Select a version of edge detection to run:", new[] {
                        "Preset - Hand Drawn Map",
                        "Preset - Screenshot",
                        "Preset - Photograph",
                        "Multi-threaded - Fast, all options decided at the start which allows for faster processing.",
                        "Synchronous - Slow, options can be changed after each step and steps can be repeated." });

                double[,] resultOfEdgeDetection = null;

                IHandler handler = opt <= 3
                            ? new AsyncEdgeDetection(menu,
                                logger,
                                rawImage,
                                runGuid)
                            : (IHandler)new SyncEdgeDetection(menu,
                                logger,
                                rawImage,
                                runGuid);

                switch (opt)
                {
                    case 0:
                        AsyncEdgeDetection handPreset = new AsyncEdgeDetection(menu,
                            logger,
                            rawImage,
                            runGuid);
                        handPreset.Preset(5, 0.299, 0.587, 0.114, 1.4, 0.1, 0.3, 1);
                        handler = handPreset;

                        break;
                    case 1:
                        AsyncEdgeDetection screenPreset = new AsyncEdgeDetection(menu,
                            logger,
                            rawImage,
                            runGuid);
                        screenPreset.Preset(5, 0.299, 0.587, 0.114, 1.4, 0.05, 0.15, 0);
                        handler = screenPreset;
                        break;
                    case 2:
                        AsyncEdgeDetection photoPreset = new AsyncEdgeDetection(menu,
                            logger,
                            rawImage,
                            runGuid);
                        photoPreset.Preset(7, 0.299, 0.587, 0.114, 2, 0.1, 0.3, 1);
                        handler = photoPreset;
                        break;
                    default:
                        handler.Start();
                        break;
                }

                resultOfEdgeDetection = handler.Result();

                //Show After to User
                ViewImageForm edgeImageForm = new ViewImageForm(resultOfEdgeDetection.ToBitmap());
                menu.WriteLine("Click and Press Enter");
                edgeImageForm.ShowDialog();

                menu.ClearUserSection();
                menu.WriteLine("In order for the road detection to function properly there must be a a box encapsulating the road. It should look like an outline of the road, if there isn't one then select invert at the next prompt.");
                menu.WriteLine();


                MapFile saveMapFile = rawImage.MapFile;

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

                // TODO Next section move road detection then graph stuff add some prompts here
                // TODO CHECK FOR REFERENCE TYPE BITMAP ISSUES
                Graph<Structures.Coord> myGraph = roadDetector.Result().PathDoubles.ToGraph();
                Traversal<Structures.Coord> myTraversal = new Traversal<Structures.Coord>(myGraph);

                PathfindImageForm myForm = new PathfindImageForm(rawImage.Original, myTraversal, myGraph);
                myForm.ShowDialog();

                logger.EndSuccessRun(runGuid);
            }
            catch (Exception ex)
            {
                menu.ClearUserSection();
                if (ex.InnerException != null) menu.Error(ex.InnerException.Message);
                else menu.Error(ex.Message);
                new Input(menu).WaitInput("");
                logger.EndError(runGuid, ex);
            }
        }
    }
}