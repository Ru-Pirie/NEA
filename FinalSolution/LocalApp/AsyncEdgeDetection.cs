using BackendLib;
using BackendLib.Interfaces;
using BackendLib.Processing;
using LocalApp.CLI;
using System;
using System.Threading.Tasks;
using Menu = LocalApp.CLI.Menu;
using ProgressBar = LocalApp.CLI.ProgressBar;

namespace LocalApp
{
    internal class AsyncEdgeDetection : IHandler
    {
        private readonly Menu m;
        private readonly Input i;
        private readonly Log l;
        private readonly Guid runGuid;
        private readonly Structures.RawImage image;
        private double[,] _resultArray;

        public AsyncEdgeDetection(Menu m, Input i, Log l, Structures.RawImage image, Guid currentGuid)
        {
            this.m = m;
            this.i = i;
            this.l = l;
            this.image = image;
            this.runGuid = currentGuid;
        }

        public void Start()
        {
            l.Event(runGuid, "Started Multi Threaded Canny Edge Detection");
            bool confirmOptions = false;
            CannyEdgeDetection detector;

            l.Event(runGuid, "Getting Multi Thread Options");

            do
            {
                detector = GetDetector(m, i, l);

                string opt = i.GetInput("Are you happy with those edge detection variables (y/n): ");
                if (opt.ToLower() == "y") confirmOptions = true;
                else m.ClearUserSection();
            } while (!confirmOptions);


            Structures.RGB[][,] quads = Utility.SplitImage(image.Pixels);
            Task<double[,]>[] threads = new Task<double[,]>[quads.Length];

            int continueOption = i.GetOption("Continue to Canny Edge Detection:", new[] { "Yes: Continue", "No: Return to main menu" });
            if (continueOption != 0) throw new Exception("Map Processing Ended At User Request");

            bool saveTempOption = i.GetOption("Would you like to save images at each step of the edge detection?", new[] { "Yes", "No" }) == 0;

            ProgressBar pb = new ProgressBar("Canny Edge Detection", 40, m);
            pb.DisplayProgress();
            
            for (int i = 0; i < quads.Length; i++)
            {
                // Overcome Capture Condition
                int copyI = i;
                Task<double[,]> task = new Task<double[,]>(() => RunDetectionOnQuadrant(detector, quads[copyI], copyI, pb.GetIncrementAction(), saveTempOption));
                task.Start();
                threads[i] = task;
            }

            Task.WaitAll(threads);
            Utility.CombineQuadrants(threads[0].Result, threads[1].Result, threads[2].Result, threads[3].Result).ToBitmap().Save("testOut.png");

            
        }

        // TODO
        private double[,] RunDetectionOnQuadrant(CannyEdgeDetection detector, Structures.RGB[,] image, int id, Action increment, bool saveTemp)
        {
            char letter = (char)('A' + id);
            double[,] _workingArray;
            l.Event(runGuid, $"Starting processing of quadrant {letter} ({id % 2}, {id / 2})");

            _workingArray = detector.BlackWhiteFilter(image);
            if (saveTemp) Logger.SaveBitmap(runGuid, _workingArray, $"BlackWhiteFilterQuad{letter}.png");
            increment();
            l.Event(runGuid, $"Completed Black and White Filter on Quadrant {letter}");

            _workingArray = detector.GaussianFilter(_workingArray);
            if (saveTemp) Logger.SaveBitmap(runGuid, _workingArray, $"GaussianFilterQuad{letter}.png");
            increment();
            l.Event(runGuid, $"Applied Gaussian Filter on Quadrant {letter}");

            Structures.Gradients grads = detector.CalculateGradients(_workingArray, increment);
            if (saveTemp)
            {
                Logger.SaveBitmap(runGuid, grads.GradientX, $"GradientXQuad{letter}.png");
                Logger.SaveBitmap(runGuid, grads.GradientY, $"GradientYQuad{letter}.png");
            }
            l.Event(runGuid, $"Calculated Gradients for Quadrant {letter}");

            double[,] _combinedGrads = detector.CombineGradients(grads);
            if (saveTemp) Logger.SaveBitmap(runGuid, _workingArray, $"CombinedGradientsQuad{letter}.png");
            increment();
            l.Event(runGuid, $"Calculated Combined Gradients for Quadrant {letter}");

            double[,] _angleGrads = detector.GradientAngle(grads);
            increment();
            // Convert to readable?
            if (saveTemp)
            {
                for (int y = 0; y < _angleGrads.GetLength(0); y++)
                for (int x = 0; x < _angleGrads.GetLength(1); x++)
                    _workingArray[y, x] = Utility.MapRadiansToPixel(_angleGrads[y, x]);

                Logger.SaveBitmap(runGuid, _workingArray, $"AngleGradientsQuad{letter}.png");
            }
            l.Event(runGuid, $"Calculated Gradient Angles for Quadrant {letter}");

            _workingArray = detector.MagnitudeThreshold(_combinedGrads, _angleGrads);
            if (saveTemp) Logger.SaveBitmap(runGuid, _workingArray, $"MagnitudeThresholdQuad{letter}.png");
            increment();
            l.Event(runGuid, $"Applied Magnitude Threshold on Quadrant {letter}");

            Structures.ThresholdPixel[,] _thresholdArray = detector.DoubleThreshold(_workingArray);
            increment();
            // just be same think about that colour strong weak etc...?
            l.Event(runGuid, $"Calculated Threshold Pixels for Quadrant {letter}");

            _workingArray = detector.EdgeTrackingHysteresis(_thresholdArray);
            if (saveTemp) Logger.SaveBitmap(runGuid, _workingArray, $"EdgeTrackingHysteresisQuad{letter}.png");
            increment();
            l.Event(runGuid, $"Applied Edge Tracking by Hysteresis on Quadrant {letter}");

            return _workingArray;
        }

        private CannyEdgeDetection GetDetector(Menu m, Input i, Log l)
        {
            CannyEdgeDetection cannyDetection = new CannyEdgeDetection();

            if (i.TryGetDouble(
                        $"Enter a value for the ratio value for red for the Black and White filter (Default: {cannyDetection.RedRatio}, Range: 0 <= x <= 1)",
                    out double newRedRatio) && newRedRatio <= 1 && newRedRatio >= 0 && newRedRatio != cannyDetection.RedRatio)
            {
                l.Warn(runGuid, $"Changed red ratio {cannyDetection.RedRatio} -> {newRedRatio}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.RedRatio} -> {newRedRatio}\x1b[0m");
                cannyDetection.RedRatio = newRedRatio;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.RedRatio}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the ratio value for green for the Black and White filter (Default: {cannyDetection.GreenRatio}, Range: 0 <= x <= 1)",
                    out double newGreenRatio) && newGreenRatio <= 1 && newGreenRatio >= 0 &&
                newGreenRatio != cannyDetection.GreenRatio)
            {
                l.Warn(runGuid, $"Changed green ratio {cannyDetection.GreenRatio} -> {newGreenRatio}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.GreenRatio} -> {newGreenRatio}\x1b[0m");
                cannyDetection.GreenRatio = newGreenRatio;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.GreenRatio}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the ratio value for blue for the Black and White filter (Default: {cannyDetection.BlueRatio}, Range: 0 <= x <= 1)",
                    out double newBlueRatio) && newBlueRatio <= 1 && newBlueRatio >= 0 && newBlueRatio != cannyDetection.BlueRatio)
            {
                l.Warn(runGuid, $"Changed blue ratio {cannyDetection.BlueRatio} -> {newBlueRatio}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.BlueRatio} -> {newBlueRatio}\x1b[0m");
                cannyDetection.BlueRatio = newBlueRatio;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.BlueRatio}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for sigma for the Gaussian Filter stage (Default: {cannyDetection.Sigma}, Range: 0 < x <= 10)",
                    out double newSigma) && newSigma <= 10 && newSigma > 0 && newSigma != cannyDetection.Sigma)
            {
                l.Warn(runGuid, $"Changed sigma value {cannyDetection.Sigma} -> {newSigma}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.Sigma} -> {newSigma}\x1b[0m");
                cannyDetection.Sigma = newSigma;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.Sigma}\x1b[0m");
            m.WriteLine();

            if (i.TryGetInt(
                    $"Enter a value for kernel size for the Gaussian Filter stage, large values will take exponentially longer (Default: {cannyDetection.KernelSize}, Range: x >= 3, x not a multiple of 2 and a whole number)",
                    out int newKernel) && newKernel >= 3 && newKernel % 2 == 1 && newKernel % 1 == 0 && newKernel != cannyDetection.KernelSize)
            {
                l.Warn(runGuid, $"Changed kernel size {cannyDetection.KernelSize} -> {newKernel}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.KernelSize} -> {newKernel}\x1b[0m");
                cannyDetection.KernelSize = newKernel;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.KernelSize}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the lower threshold for the Min Max stage (Default: {cannyDetection.LowerThreshold}, Range: 0 <= x < 1)",
                    out double newLowerThreshold) && newLowerThreshold > 0 && newLowerThreshold < 1 && newLowerThreshold != cannyDetection.LowerThreshold)
            {
                l.Warn(runGuid, $"Changed lower threshold {cannyDetection.LowerThreshold} -> {newLowerThreshold}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.LowerThreshold} -> {newLowerThreshold}\x1b[0m");
                cannyDetection.LowerThreshold = newLowerThreshold;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.LowerThreshold}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the lower threshold for the Min Max stage (Default: {cannyDetection.UpperThreshold}, Range: {cannyDetection.LowerThreshold} < x <= 1)",
                    out double newHigherThreshold) && newHigherThreshold > cannyDetection.LowerThreshold && newHigherThreshold <= 1 && newHigherThreshold != cannyDetection.UpperThreshold)
            {
                l.Warn(runGuid, $"Changed upper threshold {cannyDetection.UpperThreshold} -> {newHigherThreshold}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.UpperThreshold} -> {newHigherThreshold}\x1b[0m");
                cannyDetection.UpperThreshold = newHigherThreshold;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.UpperThreshold}\x1b[0m");
            m.WriteLine();

            i.GetInput("(Press enter to continue)");
            m.ClearUserSection();

            m.WriteLine("For reference the variables which will be used are:");
            m.WriteLine($"    Red Ratio: \x1b[38;5;2m{cannyDetection.RedRatio}\x1b[0m");
            m.WriteLine($"    Green Ratio: \x1b[38;5;2m{cannyDetection.GreenRatio}\x1b[0m");
            m.WriteLine($"    Blue Ratio: \x1b[38;5;2m{cannyDetection.BlueRatio}\x1b[0m");
            m.WriteLine($"    Gaussian Sigma Value: \x1b[38;5;2m{cannyDetection.Sigma}\x1b[0m");
            m.WriteLine($"    Gaussian Kernel Size: \x1b[38;5;2m{cannyDetection.KernelSize}\x1b[0m");
            m.WriteLine($"    Double Threshold Lower: \x1b[38;5;2m{cannyDetection.LowerThreshold}\x1b[0m");
            m.WriteLine($"    Double Threshold Upper: \x1b[38;5;2m{cannyDetection.UpperThreshold}\x1b[0m");
            m.WriteLine();

            return cannyDetection;
        }


        public double[,] Result() => _resultArray;


    }
}
