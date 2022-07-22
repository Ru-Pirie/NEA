using FinalSolution.src.utility;
using System;
using System.Drawing;
using System.IO;
using System.Text;
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

            string deletePromptInput = Prompt.GetInput("Would you like to delete the original image after processing (y/n)?");
            Menu.WriteLine();


            bool canceled = false;

            // Read in image
            Bitmap image = new Bitmap(1, 1);
            try
            {
                image = ReadAndProcessImage(filePath);
            }
            catch (Exception ex)
            {
                canceled = true;
                Log.Warn(ex.Message);
            }
            if (canceled)
            {
                Log.End("+-----------------------------+");
                Log.End("|      Process Terminated     |");
                Log.End("+-----------------------------+");
                return;
            }

            // Do edge detection
            Menu.SetPage("Start Of Canny Edge Detection");
            double[,] edgeArray = new double[1, 1];
            try
            {
                edgeArray = PerformCannyDetection(image);
            }
            catch (Exception ex)
            {
                canceled = true;
                Log.Error($"While performing edge detection: {ex.Message}");
            }
            if (canceled)
            {
                Log.End("+-----------------------------+");
                Log.End("|      Process Terminated     |");
                Log.End("+-----------------------------+");
                return;
            }

            // Do misc steps
            double[,] toFillArray = new double[1, 1];
            try
            {
                toFillArray = AlterEdgeImage(edgeArray);
            }
            catch (Exception ex)
            {
                canceled = true;
                Log.Error($"While performing image alterations: {ex.Message}");
            }
            if (canceled)
            {
                Log.End("+-----------------------------+");
                Log.End("|      Process Terminated     |");
                Log.End("+-----------------------------+");
                return;
            }

            // Do road detection
            try
            {
                PerformRoadDetection(toFillArray);
            }
            catch (Exception ex)
            {
                canceled = true;
                Log.Error($"While performing Road Detection: {ex.Message}");
            }
            if (canceled)
            {
                Log.End("+-----------------------------+");
                Log.End("|      Process Terminated     |");
                Log.End("+-----------------------------+");
                return;
            }


            if (savePromptInput.ToLower() == "y")
            {
                Log.Warn("Do save stuff here");
            }
            if (deletePromptInput.ToLower() == "y") File.Delete(filePath);



            Log.End("+-----------------------------+");
            Log.End("|      Process Completed      |");
            Log.End("+-----------------------------+");
        }

        private double[,] AlterEdgeImage(double[,] image)
        {
            int times;
            bool success = int.TryParse(Prompt.GetInput($"How many times would you like the image to be fortified, this will make the edges boulder clearer (Default: 1, Range: x >= 0, must be a whole number)?"), out times);

            if (!success) times = 1;

            double[,] toFill;
            if (times >= 0)
            {
                Menu.WriteLine($"\x1b[38;5;3mRunning Fortification {times} Time(s)\x1b[0m");
                toFill = ProcessImage.FortifyImage(image, times);
            }
            else
            {
                Menu.WriteLine($"\x1b[38;5;3mRunning Fortification 1 Time\x1b[0m");
                toFill = ProcessImage.FortifyImage(image);
            }

            Bitmap fortifiedImage = CannyEdgeDetection.DoubleArrayToBitmap(toFill);
            fortifiedImage.Save("./out/fortifiedImage.png");
            
            ShowImage fortifiedImagePrompt = new ShowImage(fortifiedImage,
                "Please take a moment to look at the image and decide whether you wish to invert it. If there are not two lines either side of the road then the filling algorithm will not be able to pick them out. If this is the case then at the next prompt please enter y at the next prompt.");
            fortifiedImagePrompt.ShowDialog();
            Menu.WriteLine();

            string invertPrompt = Prompt.GetInput("Would you like to invert this image (y/n)?");
            if (invertPrompt.ToLower() == "y")
            {
                Log.Event("Inverting Image");
                Menu.WriteLine("\x1b[38;5;2mPerforming Image Inversion\x1b[0m");
                ProcessImage.InvertImage(ref toFill);
            }
            else Menu.WriteLine("\x1b[38;5;3mSkipping Image Inversion\x1b[0m");
            Menu.WriteLine();

            string roadDetectionPrompr = Prompt.GetInput("Would you like to proceed to the Road Detection (y/n)?");
            if (roadDetectionPrompr.ToLower() != "y") throw new Exception("Map Processing Stopped after Fortification Stage before Filing Stage.");
            
            Menu.ClearUserSection();

            return toFill;
        }

        private Bitmap ReadAndProcessImage(string path)
        {
            Menu.SetPage("Parsing Image from Path");
            Log.Event("Created new instance of image processing.");
            ProcessImage processImage = new ProcessImage(path);
            processImage.Start();

            Log.Event("Finished formatting image.");
            Bitmap image = processImage.Result();

            Menu.SetPage("Confirm Edge Detection");
            Menu.WriteLine("The file path you supplied was an image and has been processed.");

            string proceed = Prompt.GetInput("Proceed to Canny Edge Detection (y/n)?");
            if (proceed.ToLower() != "y") throw new Exception("Image detection terminated at user request.");
            
            return image;
        }

        private double[,] PerformCannyDetection(Bitmap image)
        {
            CannyEdgeDetection detector = new CannyEdgeDetection(image);

            detector.Start();

            double[,] result = detector.Result();
            Menu.ClearUserSection();

            return result;
        }

        private void PerformRoadDetection(double[,] image)
        {
            Bitmap toProcess = CannyEdgeDetection.DoubleArrayToBitmap(image);
            RoadDetection roadDetection = new RoadDetection(toProcess);

            Log.Event("Starting Road Detection");

            double threshold = 0.2;

            if (double.TryParse(
                    Prompt.GetInput(
                        $"Enter a threshold value past which large blobs will be removed from the filled image (Default: 0.2, Range 0 < x < 1)"),
                    out double newRatio) && newRatio < 1 && newRatio > 0 &&
                newRatio != threshold)
            {
                Menu.WriteLine($"\x1b[38;5;2mChanged: {threshold} -> {newRatio}\x1b[0m");
                threshold = newRatio;
            }
            else Menu.WriteLine($"\x1b[38;5;3mKept Default: {threshold}\x1b[0m");
            Menu.WriteLine();

            roadDetection.Start(threshold);
            Log.End("Road Detection Finished");
        }

        private void RecallMapFromFile()
        {
            throw new NotImplementedException("This portion has not been implemented yet");
        }
    }
}
