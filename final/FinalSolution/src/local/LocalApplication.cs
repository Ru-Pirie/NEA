using FinalSolution.src.utility;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using FinalSolution.src.local.forms;

namespace FinalSolution.src.local
{
    public class LocalApplication
    {
        private bool _running = true;

        public LocalApplication()
        {
            Thread infoThread = new Thread(() => Menu.InfoBoxLoop(System.Diagnostics.Stopwatch.StartNew()));
            infoThread.Start();

            Menu.ClearUserSection();
            Log.Event("Loaded local application and started runtime counter.");

            while (_running)
            {
                Menu.SetPage("Main Menu - Please select an option");
                int opt = Prompt.GetOption("Please select an option to proceed", new[] { "Interpret a New Map Image", "Recall Map from File", "Exit Program" });
                switch (opt)
                {
                    case 0:
                        Log.Event("Beginning processing of new map image.");

                        try
                        {
                            InterpretNewImage();
                        }
                        catch (Exception e)
                        {
                            Log.Error($"While processing a new map file: {e.Message}");
                        }

                        break;
                    case 1:
                        Log.Event("Beginning retrieval of old image");

                        try
                        {
                            RecallMapFromFile();
                        }
                        catch (Exception e)
                        {
                            Log.Error($"While recalling map file: {e.Message}");
                        }

                        break;
                    case 2:
                        _running = false;
                        break;
                }
            }

            Environment.Exit(0);
        }

        private void InterpretNewImage()
        {
            Menu.SetPage("Input New Image Path");

            string filePath = Prompt.GetInput("Please enter the direct file path of the image you wish to process (you can drag and drop):");
            Menu.WriteLine();

            if (!File.Exists(filePath)) throw new Exception("No file could be found.");

            Menu.SetPage("View File Info");
            Log.Event("File found displaying details.");

            Menu.WriteLine("File information:");
            Menu.WriteLine($"    Name: {Path.GetFileNameWithoutExtension(filePath)}");
            Menu.WriteLine($"    Folder: {Path.GetDirectoryName(filePath)}");
            Menu.WriteLine($"    File extension: {Path.GetExtension(filePath)}");
            Menu.WriteLine();

            string correct = Prompt.GetInput("Is this the correct file (y/n)?");
            Menu.WriteLine();
            if (correct.ToLower() != "y")
            {
                Log.Warn("Image detection terminated at user request.");
                return;
            }

            string savePromptInput = Prompt.GetInput("Would you like to save the processed image in a binary file (y/n)?");
            Menu.WriteLine();

            bool save;
            if (savePromptInput.ToLower() == "y") save = true;

            string deletePromptInput = Prompt.GetInput("Would you like to delete the original image after processing (y/n)?");
            Menu.WriteLine();

            ReadAndProcessImage(filePath);

            if (deletePromptInput.ToLower() == "y") File.Delete(filePath);

            Log.End("+-----------------------------+");
            Log.End("|      Process Completed      |");
            Log.End("+-----------------------------+");
        }

        private void ReadAndProcessImage(string path)
        {
            Menu.SetPage("Parsing Image from Path");
            Log.Event("Created new instance of image processing.");
            ProcessImage processImage = new ProcessImage(path);
            processImage.Start();

            Log.Event("Finished formatting image.");
            Bitmap image = processImage.Result();

            Menu.SetPage("Confirm Edge Detection");
            Menu.WriteLine("The file path you supplied was an image and has been processed.");

            string proceed = Prompt.GetInput("Proceed to road detection (y/n)?");
            if (proceed.ToLower() != "y")
            {
                Log.Warn("Image detection terminated at user request.");
                return;
            }

            Menu.SetPage("Start Of Canny Edge Detection");
            Log.Event("Created new instance of canny edge detection");
            try
            {
                PerformCannyDetection(image);
            }
            catch (Exception ex)
            {
                Log.Error($"While performing edge detection: {ex.Message}");
            }
        }

        private void PerformCannyDetection(Bitmap image)
        {
            CannyEdgeDetection detector = new CannyEdgeDetection(image);

            detector.Start();

            double[,] result = detector.Result();
            Menu.ClearUserSection();

            int times;
            int.TryParse(Prompt.GetInput($"How many times would you like the image to be fortified, this will make the edges boulder clearer (Default: 1, Range: x >= 0, must be a whole number)?"), out times);

            double[,] toFill;
            if (times >= 0)
            {
                Menu.WriteLine($"\x1b[38;5;3mRunning Fortification {times} Time(s)\x1b[0m");
                toFill = ProcessImage.FortifyImage(result, times);
            }
            else
            {
                Menu.WriteLine($"\x1b[38;5;3mRunning Fortification 1 Time\x1b[0m");
                toFill = ProcessImage.FortifyImage(result);
            }

            Bitmap fortifiedImage = CannyEdgeDetection.DoubleArrayToBitmap(toFill);
            fortifiedImage.Save("./out/fortifiedImage.png");
            

            ShowImage fortifiedImagePrompt = new ShowImage(fortifiedImage,
                "Please take a moment to look at the image and decide whether you wish to invert it. If there are not two lines either side of the road then the filling algorithm will not be able to pick them out. If this is the case then at the next prompt please enter y at the next prompt.");
            fortifiedImagePrompt.ShowDialog();
            Menu.WriteLine();

            string invertPrompt = Prompt.GetInput("Would you like to invert this image (y/n)?");
            if (invertPrompt.ToLower() == "y") ProcessImage.InvertImage(ref toFill);


            ShowImage aImage = new ShowImage(CannyEdgeDetection.DoubleArrayToBitmap(toFill),
                "a");
            aImage.ShowDialog();


        }

        private void RecallMapFromFile()
        {
            throw new NotImplementedException("This portion has not been implemented yet");
        }
    }
}
