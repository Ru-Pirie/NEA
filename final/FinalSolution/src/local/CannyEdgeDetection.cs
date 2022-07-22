using FinalSolution.src.local.forms;
using FinalSolution.src.utility;
using FinalSolution.src.utility.datatypes;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace FinalSolution.src.local
{
    public class CannyEdgeDetection
    {
        private Bitmap _image;
        private Bitmap[] _quadrants;

        private readonly string _masterDir = "./cannyEdgeDetectionOut";

        private int _kernelSize = 5;
        private double _redRatio = 0.299, _greenRatio = 0.587, _blueRatio = 0.114, _sigma = 1.4, _lowerThreshold = 0.1, _upperThreshold = 0.3;
        private double[,] _output;


        public CannyEdgeDetection(Bitmap image)
        {
            Directory.CreateDirectory(_masterDir);
            Directory.CreateDirectory("out");

            _image = image;
        }

        public void Start()
        {
            Menu.SetPage("Canny Edge Detection Main Menu");
            int opt = Prompt.GetOption("Please select how you would like to run the edge detection on your image:", new[]
            {
                "Single Threaded - Slow, run with breaks at every stage (a chance to change parameters on the fly)",
                "Multi Threaded - Fast, will run with default recommended values (values set at start)"
            });

            switch (opt)
            {
                case 0:
                    Log.Warn("Single threaded mode has been selected, this could take a few minutes.");
                    
                    CreateSinglethreadSaveDirectories();
                    RunSingleThread();
                    break;
                case 1:
                    Log.Event("Multi threaded canny detection selected.");
                    Menu.WriteLine("Please wait, your image is being prepared and split into quadrants. This may take a few minutes.");
                    _quadrants = ProcessImage.SplitImage(_image);
                    Menu.WriteLine();
                    Log.Event("Image has been split into quadrants");
                    Directory.CreateDirectory("cannyEdgeDetectionOut/raw");

                    RunMultiThread();
                    break;
            }
        }

        public double[,] Result() => _output;

        private void RunSingleThread()
        {
            
            _image.Save("./out/original.png");


            bool loopStage = true;

            do
            {

            } while (loopStage);
        }

        private void RunMultiThread()
        {
            _image.Save("./out/original.png");
            Menu.WriteLine("If you do not wish to change each of the following just press enter otherwise enter a valid number otherwise it will be ignored.");

            if (double.TryParse(
                    Prompt.GetInput(
                        $"Enter a value for the ratio value for red for the Black and White filter (Default: {_redRatio}, Range: 0 <= x <= 1)"),
                    out double newRedRatio) && newRedRatio <= 1 && newRedRatio >= 0 && newRedRatio != _redRatio)
            {
                Menu.WriteLine($"\x1b[38;5;2mChanged: {_redRatio} -> {newRedRatio}\x1b[0m");
                _redRatio = newRedRatio;
            }
            else Menu.WriteLine($"\x1b[38;5;3mKept Default: {_redRatio}\x1b[0m");
            Menu.WriteLine();

            if (double.TryParse(
                    Prompt.GetInput(
                        $"Enter a value for the ratio value for green for the Black and White filter (Default: {_greenRatio}, Range: 0 <= x <= 1)"),
                    out double newGreenRatio) && newGreenRatio <= 1 && newGreenRatio >= 0 &&
                newGreenRatio != _greenRatio)
            {
                Menu.WriteLine($"\x1b[38;5;2mChanged: {_greenRatio} -> {newGreenRatio}\x1b[0m");
                _greenRatio = newGreenRatio;
            }
            else Menu.WriteLine($"\x1b[38;5;3mKept Default: {_greenRatio}\x1b[0m");
            Menu.WriteLine();

            if (double.TryParse(
                    Prompt.GetInput(
                        $"Enter a value for the ratio value for blue for the Black and White filter (Default: {_blueRatio}, Range: 0 <= x <= 1)"),
                    out double newBlueRatio) && newBlueRatio <= 1 && newBlueRatio >= 0 && newBlueRatio != _blueRatio)
            {
                Menu.WriteLine($"\x1b[38;5;2mChanged: {_blueRatio} -> {newBlueRatio}\x1b[0m");
                _blueRatio = newBlueRatio;
            }
            else Menu.WriteLine($"\x1b[38;5;3mKept Default: {_blueRatio}\x1b[0m");
            Menu.WriteLine();

            if (double.TryParse(
                    Prompt.GetInput(
                        $"Enter a value for sigma for the Gaussian Filter stage (Default: {_sigma}, Range: 0 < x <= 10)"),
                    out double newSigma) && newSigma <= 10 && newSigma > 0 && newSigma != _sigma)
            {
                Menu.WriteLine($"\x1b[38;5;2mChanged: {_sigma} -> {newSigma}\x1b[0m");
                _sigma = newSigma;
            }
            else Menu.WriteLine($"\x1b[38;5;3mKept Default: {_sigma}\x1b[0m");
            Menu.WriteLine();

            if (int.TryParse(
                    Prompt.GetInput(
                        $"Enter a value for kernel size for the Gaussian Filter stage, large values will take exponentially longer (Default: {_kernelSize}, Range: x >= 3, x not a multiple of 2 and a whole number)"),
                    out int newKernel) && newKernel >= 3 && newKernel % 2 == 1 && newKernel % 1 == 0 && newKernel != _kernelSize)
            {
                Menu.WriteLine($"\x1b[38;5;2mChanged: {_kernelSize} -> {newKernel}\x1b[0m");
                _kernelSize = newKernel;
            }
            else Menu.WriteLine($"\x1b[38;5;3mKept Default: {_kernelSize}\x1b[0m");
            Menu.WriteLine();

            if (double.TryParse(
                    Prompt.GetInput(
                        $"Enter a value for the lower threshold for the Min Max stage (Default: {_lowerThreshold}, Range: 0 <= x < 1)"),
                    out double newLowerThreshold) && newLowerThreshold > 0 && newLowerThreshold < 1 && newLowerThreshold != _lowerThreshold)
            {
                Menu.WriteLine($"\x1b[38;5;2mChanged: {_lowerThreshold} -> {newLowerThreshold}\x1b[0m");
                _lowerThreshold = newLowerThreshold;
            }
            else Menu.WriteLine($"\x1b[38;5;3mKept Default: {_lowerThreshold}\x1b[0m");
            Menu.WriteLine();

            if (double.TryParse(
                    Prompt.GetInput(
                        $"Enter a value for the lower threshold for the Min Max stage (Default: {_upperThreshold}, Range: {_lowerThreshold} < x <= 1)"),
                    out double newHigherThreshold) && newHigherThreshold > _lowerThreshold && newHigherThreshold <= 1 && newHigherThreshold != _upperThreshold)
            {
                Menu.WriteLine($"\x1b[38;5;2mChanged: {_upperThreshold} -> {newHigherThreshold}\x1b[0m");
                _upperThreshold = newHigherThreshold;
            }
            else Menu.WriteLine($"\x1b[38;5;3mKept Default: {_upperThreshold}\x1b[0m");
            Menu.WriteLine();

            Prompt.GetInput("(Press enter to continue)");
            Menu.ClearUserSection();

            Menu.WriteLine("For reference the variables which will be used are:");
            Menu.WriteLine($"    Red Ratio: \x1b[38;5;2m{_redRatio}\x1b[0m");
            Menu.WriteLine($"    Green Ratio: \x1b[38;5;2m{_greenRatio}\x1b[0m");
            Menu.WriteLine($"    Blue Ratio: \x1b[38;5;2m{_blueRatio}\x1b[0m");
            Menu.WriteLine($"    Gaussian Sigma Value: \x1b[38;5;2m{_sigma}\x1b[0m");
            Menu.WriteLine($"    Gaussian Kernel Size: \x1b[38;5;2m{_kernelSize}\x1b[0m");
            Menu.WriteLine($"    Double Threshold Lower: \x1b[38;5;2m{_lowerThreshold}\x1b[0m");
            Menu.WriteLine($"    Double Threshold Upper: \x1b[38;5;2m{_upperThreshold}\x1b[0m");
            Menu.WriteLine();

            string proceed = Prompt.GetInput($"Would you like to proceed to edge detection (y/n)?");
            if (proceed.ToLower() != "y") throw new ExitException("Program terminated at just before Edge Detection at user request.");

            CreateMultithreadSaveDirectories();

            Menu.SetupProgressBar("Performing Canny Edge Detection", 41);

            Log.Event("Beginning multithreaded edge detection.");
            Task<double[,]>[] tasks = new Task<double[,]>[4];
            for (int i = 0; i < tasks.Length; i++)
            {
                // capture condition
                int copyI = i;
                Log.Event($"Creating detection task for quadrant {i}");
                Task<double[,]> task = new Task<double[,]>(() => MultithreadedEdgeDetection(_quadrants[copyI], copyI + 1));
                task.Start();
                Log.Event($"Starting detection task for quadrant {i}");
                tasks[i] = task;
            }

            Log.Event($"Waiting for all tasks to complete...");
            Task.WaitAll(tasks);

            double[,] edgeDetectionOutput = CombineQuadrants(
                tasks[0].Result,
                tasks[1].Result,
                tasks[2].Result,
                tasks[3].Result
            );

            Menu.UpdateProgressBar();

            Bitmap edgeImage = DoubleArrayToBitmap(edgeDetectionOutput);
            edgeImage.Save($"./out/edgeDetected.png");

            Log.End("Canny Edge Detection Completed");

            ShowImage edgeImageForm = new ShowImage(edgeImage, 
                "This is the image after Canny Edge Detection, the next step is the fortification algorithm, this will make the edges more clear to the program and when it comes to the filling algorithm it will stop the edges from having breaks in them.");
            edgeImageForm.ShowDialog();

            _output = edgeDetectionOutput;
        }

        private void CreateMultithreadSaveDirectories()
        {
            Directory.CreateDirectory($"{_masterDir}/raw");
            Directory.CreateDirectory($"{_masterDir}/BWFilter");
            Directory.CreateDirectory($"{_masterDir}/GaussianFilter");
            Directory.CreateDirectory($"{_masterDir}/GradientCalculations");
            Directory.CreateDirectory($"{_masterDir}/GradientCalculations/GradX");
            Directory.CreateDirectory($"{_masterDir}/GradientCalculations/GradY");
            Directory.CreateDirectory($"{_masterDir}/GradientCalculations/Combined");
            Directory.CreateDirectory($"{_masterDir}/GradientCalculations/Theta");
            Directory.CreateDirectory($"{_masterDir}/MagnitudeThresholding");
            Directory.CreateDirectory($"{_masterDir}/MinMaxDoubleThresholding");
            Directory.CreateDirectory($"{_masterDir}/Hysteresis");
        }

        private void CreateSinglethreadSaveDirectories() => Directory.CreateDirectory("cannyEdgeDetectionOut/singleThread");


        private double[,] MultithreadedEdgeDetection(Bitmap input, int i)
        {
            Menu.UpdateProgressBar();
            Log.Event($"Saving quadrant {i} as {i}.png in {_masterDir}/raw");
            input.Save($"{_masterDir}/raw/{i}.png");
            

            Log.Event($"Converting quadrant {i} to Black and White");
            double[,] bwArray = BWFilter(input);
            Log.Event($"Saving quadrant as {i}.png in {_masterDir}/BWFilter");
            DoubleArrayToBitmap(bwArray).Save($"{_masterDir}/BWFilter/{i}.png");


            Log.Event($"Applying Gaussian Filter to quadrant {i}");
            double[,] gaussianArray = GaussianFilter(bwArray);
            Log.Event($"Saving quadrant as {i}.png in {_masterDir}/BWFilter");
            DoubleArrayToBitmap(gaussianArray).Save($"{_masterDir}/GaussianFilter/{i}.png");

            Log.Event($"Calculating Gradients of quadrant {i}");
            Task<double[,]>[] gradientArrays = CalculateGradients(gaussianArray, i);
            Log.Event($"Saving quadrant as {i}.png in {_masterDir}/GradientCalculation/GradX and /GradY");

            double[,] gradientX = gradientArrays[0].Result;
            double[,] gradientY = gradientArrays[1].Result;
            DoubleArrayToBitmap(gradientX).Save($"{_masterDir}/GradientCalculations/GradX/{i}.png");
            DoubleArrayToBitmap(gradientY).Save($"{_masterDir}/GradientCalculations/GradY/{i}.png");

            Log.Event($"Combining Gradient Calculations for quadrant {i}");
            double[,] gradientCombinedArray = CalculateGradientsCombined(gradientX, gradientY);
            Log.Event($"Saving quadrant as {i}.png in {_masterDir}/GradientCalculation/Combined/{i}.png");
            DoubleArrayToBitmap(gradientCombinedArray).Save($"{_masterDir}/GradientCalculations/Combined/{i}.jpg");

            Log.Event($"Calculating Gradient Directions for quadrant {i}");
            double[,] gradientThetaArray = CalculateGradientDirection(gradientX, gradientY);
            Log.Event($"Saving quadrant as {i}.png in {_masterDir}/GradientCalculation/Theta/{i}.png");
            ConvertThetaToBitmap(gradientThetaArray).Save($"{_masterDir}/GradientCalculations/Theta/{i}.jpg");
            

            Log.Event($"Applying Magnitude Thresholding on quadrant {i}");
            double[,] gradientMagnitudeThresholdArray = ApplyGradientMagnitudeThreshold(gradientThetaArray, gradientCombinedArray);
            Log.Event($"Saving quadrant as {i}.png in {_masterDir}/MagnitudeThresholding/{i}.png");
            DoubleArrayToBitmap(gradientMagnitudeThresholdArray).Save($"{_masterDir}/MagnitudeThresholding/{i}.jpg");
            

            Log.Event($"Applying Secondary Min Max Thresholding on quadrant {i}");
            (double, bool)[,] doubleThresholdArray = ApplyDoubleThreshold(gradientMagnitudeThresholdArray);
            Log.Event($"Saving quadrant as {i}.png in {_masterDir}/MinMaxDoubleThresholding/{i}.png");
            
            double[,] doubleThresholdImageArray = new double[input.Height, input.Width];
            for (int y = 0; y < input.Height; y++) for (int x = 0; x < input.Width; x++) doubleThresholdImageArray[y, x] = doubleThresholdArray[y, x].Item1;
            DoubleArrayToBitmap(doubleThresholdImageArray).Save($"{_masterDir}/MinMaxDoubleThresholding/{i}.jpg");

            Log.Event($"Applying Edge Tracking Hysteresis on quadrant {i}");
            double[,] hysteresisArray = ApplyEdgeTrackingHysteresis(doubleThresholdArray);
            Log.Event($"Saving quadrant as {i}.png in {_masterDir}/Hysteresis/{i}.png");
            DoubleArrayToBitmap(hysteresisArray).Save($"{_masterDir}/Hysteresis/{i}.jpg");
            

            return hysteresisArray;
        }

        private static Bitmap ConvertThetaToBitmap(double[,] angles)
        {
            Bitmap image = new Bitmap(angles.GetLength(1), angles.GetLength(0));

            for (int i = 0; i < angles.GetLength(0); i++)
            {
                for (int j = 0; j < angles.GetLength(1); j++)
                {
                    int x = (int)(
                        ((128 / (2 * Math.PI)) * angles[i, j]) + 128
                    );

                    image.SetPixel(j, i, Color.FromArgb(x, x, x));
                }
            }

            return image;
        }

        private double[,] ApplyEdgeTrackingHysteresis((double, bool)[,] arrayOfValues)
        {
            double[,] result = new double[arrayOfValues.GetLength(0), arrayOfValues.GetLength(1)];

            for (int i = 0; i < arrayOfValues.GetLength(0); i++)
            {
                for (int j = 0; j < arrayOfValues.GetLength(1); j++)
                {
                    if (arrayOfValues[i, j].Item2 == false)
                    {
                        (double, bool)[,] imageKernel = BuildKernel(j, i, 3, arrayOfValues);
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (imageKernel[k, l].Item2 == false) result[i, j] = 0;
                                else result[i, j] = 255;
                            }
                        }
                    }
                    else result[i, j] = 255;
                }
            }

            Menu.UpdateProgressBar();
            return result;
        }

        private (double, bool)[,] ApplyDoubleThreshold(double[,] gradients)
        {
            double min = _lowerThreshold * 255;
            double max = _upperThreshold * 255;

            (double, bool)[,] result = new (double, bool)[gradients.GetLength(0), gradients.GetLength(1)];

            for (int i = 0; i < gradients.GetLength(0); i++)
            {
                for (int j = 0; j < gradients.GetLength(1); j++)
                {
                    if (gradients[i, j] < min) result[i, j] = (0, false);
                    else if (gradients[i, j] > min && gradients[i, j] < max) result[i, j] = (gradients[i, j], false);
                    else if (gradients[i, j] > max) result[i, j] = (gradients[i, j], true);
                    else throw new Exception();
                }
            }

            Menu.UpdateProgressBar();
            return result;
        }

        private double[,] ApplyGradientMagnitudeThreshold(double[,] angles, double[,] magnitudes)
        {
            double[,] result = magnitudes;
            double[,] anglesInDegrees = ConvertThetaToDegrees(angles);

            for (int i = 0; i < anglesInDegrees.GetLength(0); i++)
            {
                for (int j = 0; j < anglesInDegrees.GetLength(1); j++)
                {
                    double[,] magnitudeKernel = BuildKernel(j, i, 3, magnitudes).matrix;

                    if (anglesInDegrees[i, j] < 22.5 || anglesInDegrees[i, j] >= 157.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[1, 2] || magnitudes[i, j] < magnitudeKernel[1, 0]) result[i, j] = 0;
                    }
                    else if (anglesInDegrees[i, j] >= 22.5 && anglesInDegrees[i, j] < 67.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 2] || magnitudes[i, j] < magnitudeKernel[2, 0]) result[i, j] = 0;
                    }
                    else if (anglesInDegrees[i, j] >= 67.5 && anglesInDegrees[i, j] < 112.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 1] || magnitudes[i, j] < magnitudeKernel[2, 1]) result[i, j] = 0;
                    }
                    else if (anglesInDegrees[i, j] >= 112.5 && anglesInDegrees[i, j] < 157.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 0] || magnitudes[i, j] < magnitudeKernel[2, 2]) result[i, j] = 0;
                    }
                    else throw new Exception();
                }
            }

            Menu.UpdateProgressBar();
            return result;
        }

        private double[,] ConvertThetaToDegrees(double[,] thetaArray)
        {
            double[,] result = new double[thetaArray.GetLength(0), thetaArray.GetLength(1)];
            for (int i = 0; i < thetaArray.GetLength(0); i++) for (int j = 0; j < thetaArray.GetLength(1); j++) result[i, j] = 180 * Math.Abs(thetaArray[i, j]) / Math.PI;
            return result;
        }

        // Implement Atan2 myself?
        private double[,] CalculateGradientDirection(double[,] gradX, double[,] gradY)
        {
            double[,] result = new double[gradX.GetLength(0), gradX.GetLength(1)];
            for (int i = 0; i < gradX.GetLength(0); i++) for (int j = 0; j < gradX.GetLength(1); j++) result[i, j] = Math.Atan2(gradY[i, j], gradX[i, j]);

            Menu.UpdateProgressBar();
            return result;
        }

        private double[,] CalculateGradientsCombined(double[,] gradX, double[,] gradY)
        {
            double[,] result = new double[gradX.GetLength(0), gradX.GetLength(1)];
            for (int i = 0; i < gradX.GetLength(0); i++) for (int j = 0; j < gradX.GetLength(1); j++) result[i, j] = Math.Sqrt(Math.Pow(gradX[i, j], 2) + Math.Pow(gradY[i, j], 2));

            Menu.UpdateProgressBar();
            return result;
        }

        private double[,] CalculateGradientX(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix sobelX = new Matrix(new double[,] {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1 },
            });

            for (int i = 0; i < imageArray.GetLength(0); i++)
            {
                for (int j = 0; j < imageArray.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, 3, imageArray);
                    result[i, j] = Matrix.Convolution(imageKernel, sobelX);
                }
            }

            Menu.UpdateProgressBar();
            return result;
        }

        private double[,] CalculateGradientY(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix sobelY = new Matrix(new double[,] {
                {  1,  2,  1 },
                {  0,  0,  0 },
                { -1, -2, -1 },
            });
            for (int i = 0; i < imageArray.GetLength(0); i++)
            {
                for (int j = 0; j < imageArray.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, 3, imageArray);
                    result[i, j] = Matrix.Convolution(imageKernel, sobelY);
                }
            }

            Menu.UpdateProgressBar();
            return result;
        }

        private Task<double[,]>[] CalculateGradients(double[,] image, int i)
        {
            Task<double[,]>[] tasks =
            {
                new Task<double[,]>(() => CalculateGradientX(image)),
                new Task<double[,]>(() => CalculateGradientY(image))
            };

            for (int j = 0; j < tasks.Length; j++)
            {
                char type = j == 0 ? 'X' : 'Y';
                Log.Event($"Starting gradient calculation in {type} for quadrant {i}");
                tasks[j].Start();
            }

            Task.WaitAll(tasks);

            return tasks;
        }

        private double[,] GaussianFilter(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix gaussianKernel = GetGaussianKernel();

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, _kernelSize, imageArray);
                    double sum = Matrix.Convolution(imageKernel, gaussianKernel);
                    result[i, j] = sum;
                }
            }

            Menu.UpdateProgressBar();
            return result;
        }

        private Matrix GetGaussianKernel()
        {
            double[,] result = new double[_kernelSize, _kernelSize];
            int halfK = _kernelSize / 2;

            double sum = 0;

            int cntY = -halfK;
            for (int i = 0; i < _kernelSize; i++)
            {
                int cntX = -halfK;
                for (int j = 0; j < _kernelSize; j++)
                {
                    result[halfK + cntY, halfK + cntX] = GetGaussianDistribution(cntX, cntY, _sigma);
                    sum += result[halfK + cntY, halfK + cntX];
                    cntX++;
                }
                cntY++;
            }

            for (int i = 0; i < _kernelSize; i++) for (int j = 0; j < _kernelSize; j++) result[i, j] /= sum;
            return new Matrix(result);
        }

        private double[,] BWFilter(Bitmap image)
        {
            double[,] result = new double[image.Height, image.Width];

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color c = image.GetPixel(j, i);
                    double value = c.R * _redRatio + c.G * _greenRatio + c.B * _blueRatio;

                    result[i, j] = Bound(0, 255, value);
                }
            }

            Menu.UpdateProgressBar();
            return result;
        }

        private double[,] CombineQuadrants(double[,] a, double[,] b, double[,] c, double[,] d)
        {
            double[,] partA = new double[_image.Height / 2, _image.Width];
            double[,] partB = new double[_image.Height / 2, _image.Width];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                    partA[i, j] = a[i, j];

                for (int y = 0; y < b.GetLength(1); y++)
                    partA[i, y + a.GetLength(1)] = b[i, y];
            }

            for (int i = 0; i < c.GetLength(0); i++)
            {
                for (int j = 0; j < c.GetLength(1); j++)
                    partB[i, j] = c[i, j];

                for (int y = 0; y < d.GetLength(1); y++)
                    partB[i, y + c.GetLength(1)] = d[i, y];
            }

            double[,] final = new double[_image.Height, _image.Width];
            for (int i = 0; i < _image.Height; i++)
            {
                if (i < _image.Height / 2)
                {
                    for (int j = 0; j < _image.Width; j++)
                    {
                        final[i, j] = partA[i, j];
                    }
                }
                else
                {
                    for (int j = 0; j < _image.Width; j++)
                    {
                        final[i, j] = partB[i - _image.Height / 2, j];
                    }
                }
            }

            return final;
        }

        public static Bitmap DoubleArrayToBitmap(double[,] input)
        {
            Bitmap image = new Bitmap(input.GetLength(1), input.GetLength(0));
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    int val = Bound(0, 255, input[i, j]);
                    image.SetPixel(j, i, Color.FromArgb(val, val, val));
                }
            }
            return image;
        }

        public double GetGaussianDistribution(int x, int y, double sigma) =>
            1 / (2 * Math.PI * sigma * sigma) * Math.Exp(-((Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * sigma * sigma)));
        public static int Bound(int l, int h, double v) => v > h ? h : v < l ? l : (int)v;

        public static Matrix BuildKernel(int x, int y, int k, double[,] grid)
        {
            double[,] kernel = new double[k, k];

            int halfK = k / 2;

            for (int i = 0; i < k; i++) for (int j = 0; j < k; j++) kernel[i, j] = grid[y, x];

            int cntY = 0;
            for (int j = y - halfK; j <= y + halfK; j++)
            {
                int cntX = 0;
                for (int i = x - halfK; i <= x + halfK; i++)
                {
                    if (j >= 0 && i >= 0 && j < grid.GetLength(0) && i < grid.GetLength(1))
                    {
                        kernel[cntY, cntX] = grid[j, i];
                    }
                    cntX++;
                }
                cntY++;
            }

            return new Matrix(kernel);
        }

        private (double, bool)[,] BuildKernel(int x, int y, int k, (double, bool)[,] grid)
        {
            (double, bool)[,] kernel = new (double, bool)[k, k];

            int halfK = k / 2;

            for (int i = 0; i < k; i++) for (int j = 0; j < k; j++) kernel[i, j] = grid[y, x];

            int cntY = 0;
            for (int j = y - halfK; j <= y + halfK; j++)
            {
                int cntX = 0;
                for (int i = x - halfK; i <= x + halfK; i++)
                {
                    if (j >= 0 && i >= 0 && j < grid.GetLength(0) && i < grid.GetLength(1))
                    {
                        kernel[cntY, cntX] = grid[j, i];
                    }
                    cntX++;
                }
                cntY++;
            }

            return kernel;
        }
    }
}