using BackendLib;
using BackendLib.Data;
using BackendLib.Datatypes;
using BackendLib.Interfaces;
using LocalApp.CLI;
using LocalApp.Processes;
using LocalApp.WindowsForms;
using System;
using System.IO;

//TODO in the morning add page changes

namespace LocalApp
{
    public class Program
    {
        private static void Main()
        {
            Menu menu = new Menu("Author: Rubens Pirie", $"{Log.Grey}Production Mode{Log.Blank}");
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
                        "Process New Image Into Map Data File", "Recall Map From Data File", "Settings", "Exit Program"
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

                bool running = true;

                while (running)
                {
                    int opt = inputHandel.GetOption("What would you like to do with your recalled map?",
                        new[]
                        {
                            "View File / Map Information",
                            "Change File Information",
                            "Clone File",
                            "Rename File",
                            "Delete File",
                            "Pathfind Through Image",
                            "Exit"
                        });

                    switch (opt)
                    {
                        case 0:
                            string[] items = { "Screenshot", "Hand Drawn", "Photograph", "Other" };
                            menu.ClearUserSection();
                            menu.WriteLine("Your current save file information:");
                            menu.WriteLine($"Name: {recalledMap.Name}");
                            menu.WriteLine($"Description: {recalledMap.Description}");
                            menu.WriteLine();
                            menu.WriteLine($"Type of image: {Log.Orange}{items[recalledMap.Type]}{Log.Blank}");
                            menu.WriteLine($"Was it inverted: {Log.Purple}{(recalledMap.IsInverted ? "Yes" : "No")}{Log.Blank}");
                            menu.WriteLine($"Time Created: {Log.Green}{recalledMap.TimeCreated}{Log.Grey}");
                            inputHandel.WaitInput($"{Log.Grey}(Enter to Continue){Log.Blank}");
                            break;
                        case 1:
                            int option = inputHandel.GetOption("What part of the tile information do you wish to change:",
                                new[] { "1. Name", "2. Description", "3. Type of image" });
                            logger.Event(runGuid, $"Changing file settings, see current run save folder for the save file.");
                            switch (option)
                            {
                                case 0:
                                    string newName =
                                        inputHandel.GetInput("What do you want to change the title of the save to?");
                                    recalledMap.Name = newName;
                                    break;
                                case 1:
                                    string newDescription =
                                        inputHandel.GetInput("What do you want to change the title of the save to?");
                                    recalledMap.Description = newDescription;
                                    break;
                                case 2:
                                    recalledMap.Type = inputHandel.GetOption("What type of image is this save?",
                                        new[] { "Screenshot", "Hand Drawn", "Photograph", "Other" });
                                    break;
                            }

                            string path = recalledMap.Save(runGuid);
                            if (bool.Parse(Settings.UserSettings["shortNames"]
                                    .Item1))
                                File.Move(path,
                                    path.Replace(Path.GetFileName(path)
                                            .Split('.')[0],
                                        recalledMap.Name));
                            break;
                        case 2:
                            File.Copy(recalledMap._filePath, recalledMap._filePath.Replace(Path.GetFileName(recalledMap._filePath).Split('.')[0], Path.GetFileName(recalledMap._filePath).Split('.')[0] + "-CLONE"));
                            logger.Event($"Cloned {recalledMap._filePath}.");
                            break;
                        case 3:
                            string name = inputHandel.GetInput("What would you like to rename the file too?");
                            logger.Event(runGuid, $"Renamed {Path.GetFileName(recalledMap._filePath).Split('.')[0]} to {name}.");
                            File.Move(recalledMap._filePath, recalledMap._filePath.Replace(Path.GetFileName(recalledMap._filePath).Split('.')[0], name));
                            break;
                        case 4:
                            if (inputHandel.GetOption("Are you sure you want to delete the save?",
                                    new[] { $"{Log.Red}No{Log.Blank}", $"{Log.Red}No{Log.Blank}", $"{Log.Green}Yes{Log.Blank}", $"{Log.Red}No{Log.Blank}", $"{Log.Red}No{Log.Blank}" }) == 2)
                            {
                                logger.Warn(runGuid, $"Save file at path {recalledMap._filePath} deleted.");
                                File.Delete(recalledMap._filePath);
                                running = false;
                            }
                            break;
                        case 5:
                            logger.Event(runGuid, $"Starting pathfinding of recalled image.");
                            double[,] doubles = recalledMap.PathImage.ToDoubles(Utility.GetIfExists);
                            new Pathfinder(recalledMap.OriginalImage, doubles).Start();
                            break;
                        default:
                            running = false;
                            break;
                    }
                }
                
                logger.EndSuccessSave(runGuid);
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

                menu.WriteLine();
                menu.WriteLine("Successfully managed to read in your image, please look carefully at the next popup and make sure it is your image.");
                i.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");
                menu.WriteLine();

                logger.Event(runGuid, $"Confirming is correct file.");
                ViewImageForm beforeForm = new ViewImageForm(rawImage.Pixels.ToBitmap());
                beforeForm.ShowDialog();
                menu.ClearUserSection();

                TextWall.FileDetails(menu, rawImage);
                menu.WriteLine();

                bool correctImage = Utility.IsYes(i.GetInput("Is this the correct image (y/n)?"));
                if (!correctImage) throw new Exception("You asked for the processing of your map to stop.");

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
                        handPreset.Preset(5, 0.299, 0.587, 0.114, 2, 0.07, 0.25, 2);
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

                menu.ClearUserSection();
                menu.WriteLine("In order for the road detection to function properly there must be a outline encapsulating the road. It should look like an outline of the road, if there isn't one, and there is just a big white blob then select invert at the next prompt.");
                menu.WriteLine();
                i.WaitInput($"{Log.Grey}(Enter to continue){Log.Blank}");
                menu.WriteLine();

                ViewImageForm edgeImageForm = new ViewImageForm(resultOfEdgeDetection.ToBitmap());
                edgeImageForm.ShowDialog();

                MapFile saveMapFile = rawImage.MapFile;

                RoadSequence roadDetector = new RoadSequence(menu, logger, runGuid, resultOfEdgeDetection, saveMapFile);
                roadDetector.Start();

                new Pathfinder(rawImage.Original, roadDetector.Result().PathDoubles).Start();

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