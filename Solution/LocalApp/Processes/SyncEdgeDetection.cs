using BackendLib;
using BackendLib.Interfaces;
using BackendLib.Processing;
using LocalApp.CLI;
using LocalApp.WindowsForms;
using System;
using System.Drawing;

namespace LocalApp
{
    internal class SyncEdgeDetection : IHandler
    {
        private readonly Menu _menuInstance;
        private readonly Log _logInstance;
        private Input _classInputHandel;
        private readonly Structures.RawImage _image;
        private readonly Guid _runGuid;
        private double[,] _workingArray;
        private double[,] _resultArray;
        private CannyEdgeDetection _detector;

        public SyncEdgeDetection(Menu menu, Log logger, Structures.RawImage image, Guid currentGuid)
        {
            _menuInstance = menu;
            _logInstance = logger;
            _image = image;
            _runGuid = currentGuid;
        }

        public void Start()
        {
            _classInputHandel = new Input(_menuInstance);
            _detector = new CannyEdgeDetection();

            ShowDialog();
            BlackWhiteStep();
            GaussianStep();

            _menuInstance.WriteLine("The next 5 steps don't require any parameters, you will still see the result of each step however, in the order of:");
            _menuInstance.WriteLine("   1. Gradient in X");
            _menuInstance.WriteLine("   2. Gradient in Y");
            _menuInstance.WriteLine("   3. Combined Gradients");
            _menuInstance.WriteLine("   4. Gradient Directions");
            _menuInstance.WriteLine("   5. Magnitude Threshold");
            _menuInstance.WriteLine();
            _classInputHandel.WaitInput($"{Log.Grey}(Enter to Continue){Log.Blank}");
            _menuInstance.WriteLine("This may take some time to process each step.");

            Structures.Gradients grads = _detector.CalculateGradients(_workingArray, () => { });
            ViewImageForm gradXForm = new ViewImageForm(grads.GradientX.ToBitmap());
            gradXForm.ShowDialog();

            ViewImageForm gradYForm = new ViewImageForm(grads.GradientY.ToBitmap());
            gradYForm.ShowDialog();

            _workingArray = _detector.CombineGradients(grads);
            ViewImageForm combinedGradientForm = new ViewImageForm(_workingArray.ToBitmap());
            combinedGradientForm.ShowDialog();

            double[,] gradientDirections = _detector.GradientAngle(grads);
            double[,] gradCopy = gradientDirections;
            for (int y = 0; y < gradientDirections.GetLength(0); y++)
                for (int x = 0; x < gradientDirections.GetLength(1); x++)
                    gradCopy[y, x] = Utility.MapRadiansToPixel(gradientDirections[y, x]);
            ViewImageForm gradientDirectionForm = new ViewImageForm(gradCopy.ToBitmap());
            gradientDirectionForm.ShowDialog();

            _workingArray = _detector.MagnitudeThreshold(_workingArray, gradientDirections);
            ViewImageForm magnitudeForm = new ViewImageForm(_workingArray.ToBitmap());
            magnitudeForm.ShowDialog();

            _menuInstance.ClearUserSection();

            Structures.ThresholdPixel[,] _thresholdPixels = DoubleThresholdStep();

            _menuInstance.WriteLine("From here on out stages are automated, however as before you will see each step after it occurs.");
            _menuInstance.WriteLine();
            _classInputHandel.WaitInput($"{Log.Grey}(Enter to Continue){Log.Blank}");

            _workingArray = _detector.EdgeTrackingHysteresis(_thresholdPixels);
            ViewImageForm edgeTrackingForm = new ViewImageForm(_workingArray.ToBitmap());
            edgeTrackingForm.ShowDialog();

            PostProcessImage(_workingArray);
        }

        private void PostProcessImage(double[,] image)
        {
            Post postProcessor = new Post(image);

            _menuInstance.ClearUserSection();
            if (_classInputHandel.TryGetInt("How many times would you like to emboss the image (can be 0): ", out int loopCount) &&
                loopCount > 0)
            {
                _menuInstance.WriteLine();
                _menuInstance.WriteLine($"Running image embossing this will take approximately {Log.Red}{10 * loopCount}{Log.Blank} seconds!");
                postProcessor.Start(loopCount);
            }
            else
            {
                _menuInstance.WriteLine();
                _menuInstance.WriteLine($"Running image embossing this will take approximately {Log.Red}10{Log.Blank} seconds!");
                postProcessor.Start(0);
            }

            _resultArray = postProcessor.Result();
        }

        private Structures.ThresholdPixel[,] DoubleThresholdStep()
        {
            bool happy = false;
            Structures.ThresholdPixel[,] _workingThresholdPixels = new Structures.ThresholdPixel[0, 0];

            _menuInstance.WriteLine($"The 8th stage of Canny Edge Detection is applying a double threshold. It is made up of two parameters a lower and upper threshold.");

            while (!happy)
            {
                if (_classInputHandel.TryGetDouble(
                        $"Value for Lower Threshold (Default: {_detector.LowerThreshold}, Range: 0 <= x < 1)",
                        out double newLowerThreshold) && newLowerThreshold > 0 && newLowerThreshold < 1 && newLowerThreshold != _detector.LowerThreshold)
                {
                    _logInstance.Warn(_runGuid, $"Changed lower threshold {_detector.LowerThreshold} -> {newLowerThreshold}");
                    _menuInstance.WriteLine($"{Log.Green}Changed: {_detector.LowerThreshold} -> {newLowerThreshold}{Log.Blank}");
                    _detector.LowerThreshold = newLowerThreshold;
                }
                else _menuInstance.WriteLine($"{Log.Orange}Kept Default: {_detector.LowerThreshold}{Log.Blank}");
                _menuInstance.WriteLine();

                if (_classInputHandel.TryGetDouble(
                        $"Value for Upper Threshold  (Default: {_detector.UpperThreshold}, Range: {_detector.LowerThreshold} < x <= 1)",
                        out double newHigherThreshold) && newHigherThreshold > _detector.LowerThreshold && newHigherThreshold <= 1 && newHigherThreshold != _detector.UpperThreshold)
                {
                    _logInstance.Warn(_runGuid, $"Changed upper threshold {_detector.UpperThreshold} -> {newHigherThreshold}");
                    _menuInstance.WriteLine($"{Log.Green}Changed: {_detector.UpperThreshold} -> {newHigherThreshold}{Log.Blank}");
                    _detector.UpperThreshold = newHigherThreshold;
                }
                else _menuInstance.WriteLine($"{Log.Orange}Kept Default: {_detector.UpperThreshold}{Log.Blank}");
                _menuInstance.WriteLine();
                _menuInstance.WriteLine();
                _menuInstance.WriteLine("Applying Double Threshold. This may take some time...");

                _workingThresholdPixels = _detector.DoubleThreshold(_workingArray);
                Bitmap toView = new Bitmap(_workingThresholdPixels.GetLength(1), _workingThresholdPixels.GetLength(0));
                for (int y = 0; y < _workingThresholdPixels.GetLength(0); y++)
                {
                    for (int x = 0; x < _workingThresholdPixels.GetLength(1); x++)
                    {
                        if (_workingThresholdPixels[y, x].Strong) toView.SetPixel(x, y, Color.Green);
                        else if (!_workingThresholdPixels[y, x].Strong && _workingThresholdPixels[y, x].Value != 0) toView.SetPixel(x, y, Color.Red);
                        else toView.SetPixel(x, y, Color.Black);
                    }
                }
                ViewImageForm gaussianForm = new ViewImageForm(toView);
                _menuInstance.ClearUserSection();
                gaussianForm.ShowDialog();

                _menuInstance.WriteLine("Current values for thresholds");
                _menuInstance.WriteLine($"Lower: {_detector.LowerThreshold}");
                _menuInstance.WriteLine($"Upper: {_detector.UpperThreshold}");
                _menuInstance.WriteLine();

                string opt = _classInputHandel.GetInput("Are you happy with these values for the upper and lower threshold (y/n)?");

                if (opt.ToLower().StartsWith("y")) happy = true;
                else
                {
                    _menuInstance.ClearUserSection();
                    _menuInstance.WriteLine($"{Log.Pink}Please re-enter your values.{Log.Blank}");
                }
            }

            _menuInstance.ClearUserSection();
            return _workingThresholdPixels;
        }

        private void GaussianStep()
        {
            bool happy = false;

            _menuInstance.WriteLine($"The second stage of Canny Edge Detection is applying a Gaussian filter. It is made up of two parameters sigma and kernel size.");

            while (!happy)
            {
                if (_classInputHandel.TryGetDouble(
                        $"Value for Sigma (Default: {_detector.Sigma}, Range: 0 < x <= 10)",
                        out double newSigma) && newSigma <= 10 && newSigma > 0 && newSigma != _detector.Sigma)
                {
                    _logInstance.Warn(_runGuid, $"Changed Sigma value {_detector.Sigma} -> {newSigma}");
                    _menuInstance.WriteLine($"{Log.Green}Changed: {_detector.Sigma} -> {newSigma}{Log.Blank}");
                    _detector.Sigma = newSigma;
                }
                else _menuInstance.WriteLine($"{Log.Orange}Kept Default: {_detector.Sigma}{Log.Blank}");
                _menuInstance.WriteLine();

                if (_classInputHandel.TryGetInt(
                        $"Value for Kernel Size (Default: {_detector.KernelSize}, Range: x >= 3, x not a multiple of 2 and a whole number)",
                        out int newKernel) && newKernel >= 3 && newKernel % 2 == 1 && newKernel % 1 == 0 && newKernel != _detector.KernelSize)
                {
                    _logInstance.Warn(_runGuid, $"Changed Kernel Size {_detector.KernelSize} -> {newKernel}");
                    _menuInstance.WriteLine($"{Log.Green}Changed: {_detector.KernelSize} -> {newKernel}{Log.Blank}");
                    _detector.KernelSize = newKernel;
                }
                else _menuInstance.WriteLine($"{Log.Orange}Kept Default: {_detector.KernelSize}{Log.Blank}");
                _menuInstance.WriteLine();
                _menuInstance.WriteLine("Applying Gaussian Filter. This may take some time...");

                _workingArray = _detector.GaussianFilter(_workingArray);
                ViewImageForm gaussianForm = new ViewImageForm(_workingArray.ToBitmap());
                _menuInstance.ClearUserSection();
                gaussianForm.ShowDialog();

                _menuInstance.WriteLine("Current values");
                _menuInstance.WriteLine($"Sigma: {_detector.Sigma}");
                _menuInstance.WriteLine($"Kernel Size: {_detector.KernelSize}");
                _menuInstance.WriteLine();

                string opt = _classInputHandel.GetInput("Are you happy with this value of sigma and the result (y/n)?");

                if (opt.ToLower().StartsWith("y")) happy = true;
                else
                {
                    _menuInstance.ClearUserSection();
                    _menuInstance.WriteLine($"{Log.Pink}Please re-enter your values.{Log.Blank}");
                }
            }

            _menuInstance.ClearUserSection();
        }

        private void BlackWhiteStep()
        {
            bool happy = false;

            _menuInstance.WriteLine($"The first stage of Canny Edge Detection is the Black and White filter. It is made up of 3 parameters {Log.Red}Red{Log.Blank}, {Log.Green}Green{Log.Blank}, {Log.Blue}Blue{Log.Blank} Ratios.");

            while (!happy)
            {
                if (_classInputHandel.TryGetDouble(
                        $"Value for {Log.Red}Red{Log.Blank} (Old: {_detector.RedRatio}, Range: 0 <= x <= 1)",
                        out double newRedRatio) && newRedRatio <= 1 && newRedRatio >= 0 && newRedRatio != _detector.RedRatio)
                {
                    _logInstance.Warn(_runGuid, $"Changed {Log.Red}Red{Log.Blank} ratio {_detector.RedRatio} -> {newRedRatio}");
                    _menuInstance.WriteLine($"{Log.Green}Changed: {_detector.RedRatio} -> {newRedRatio}{Log.Blank}");
                    _detector.RedRatio = newRedRatio;
                }
                else _menuInstance.WriteLine($"{Log.Orange}Kept Default: {_detector.RedRatio}{Log.Blank}");
                _menuInstance.WriteLine();

                if (_classInputHandel.TryGetDouble(
                            $"Value for {Log.Green}Green{Log.Blank} (Old: {_detector.GreenRatio}, Range: 0 <= x <= 1)",
                        out double newGreenRatio) && newGreenRatio <= 1 && newGreenRatio >= 0 &&
                    newGreenRatio != _detector.GreenRatio)
                {
                    _logInstance.Warn(_runGuid, $"Changed {Log.Green}Green{Log.Blank} ratio {_detector.GreenRatio} -> {newGreenRatio}");
                    _menuInstance.WriteLine($"{Log.Green}Changed: {_detector.GreenRatio} -> {newGreenRatio}{Log.Blank}");
                    _detector.GreenRatio = newGreenRatio;
                }
                else _menuInstance.WriteLine($"{Log.Orange}Kept Default: {_detector.GreenRatio}{Log.Blank}");
                _menuInstance.WriteLine();

                if (_classInputHandel.TryGetDouble(
                            $"Value for {Log.Blue}Blue{Log.Blank} (Old: {_detector.BlueRatio}, Range: 0 <= x <= 1)",
                        out double newBlueRatio) && newBlueRatio <= 1 && newBlueRatio >= 0 && newBlueRatio != _detector.BlueRatio)
                {
                    _logInstance.Warn(_runGuid, $"Changed {Log.Blue}Blue{Log.Blank} ratio {_detector.BlueRatio} -> {newBlueRatio}");
                    _menuInstance.WriteLine($"{Log.Green}Changed: {_detector.BlueRatio} -> {newBlueRatio}{Log.Blank}");
                    _detector.BlueRatio = newBlueRatio;
                }
                else _menuInstance.WriteLine($"{Log.Orange}Kept Default: {_detector.BlueRatio}{Log.Blank}");
                _menuInstance.WriteLine();
                _menuInstance.WriteLine("Converting to black and white. This may take some time...");

                _workingArray = _detector.BlackWhiteFilter(_image.Pixels);
                ViewImageForm blackWhiteForm = new ViewImageForm(_workingArray.ToBitmap());
                _menuInstance.ClearUserSection();
                blackWhiteForm.ShowDialog();

                _menuInstance.WriteLine("Current values for ratios");
                _menuInstance.WriteLine($"Red: {Log.Red}{_detector.RedRatio}{Log.Blank}");
                _menuInstance.WriteLine($"Green: {Log.Green}{_detector.GreenRatio}{Log.Blank}");
                _menuInstance.WriteLine($"Blue: {Log.Blue}{_detector.BlueRatio}{Log.Blank}");
                _menuInstance.WriteLine();

                string opt = _classInputHandel.GetInput("Are you happy with these values and the result (y/n)?");

                if (opt.ToLower().StartsWith("y")) happy = true;
                else
                {
                    _menuInstance.ClearUserSection();
                    _menuInstance.WriteLine($"{Log.Pink}Please re-enter your values.{Log.Blank}");
                }
            }
            _menuInstance.ClearUserSection();
        }

        private void ShowDialog()
        {
            _menuInstance.ClearUserSection();
            _menuInstance.WriteLine("You have selected to run edge detection steps one after another, this means that at the end of every step you will be shown your image and then have the option to continue to the next step or change variables.");
            _classInputHandel.WaitInput($"{Log.Grey}(Enter to Continue){Log.Blank}");
            _menuInstance.WriteLine();
        }

        public double[,] Result() => _resultArray;
    }
}
