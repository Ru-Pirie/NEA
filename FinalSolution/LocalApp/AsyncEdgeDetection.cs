﻿using BackendLib;
using BackendLib.Interfaces;
using BackendLib.Processing;
using LocalApp.CLI;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Menu = LocalApp.CLI.Menu;
using ProgressBar = LocalApp.CLI.ProgressBar;

namespace LocalApp
{
    internal class AsyncEdgeDetection : IHandler
    {
        private readonly Menu _m;
        private readonly Input _i;
        private readonly Log _l;
        private readonly Guid _runGuid;
        private readonly Structures.RawImage _image;
        private double[,] _resultArray;

        public AsyncEdgeDetection(Menu m, Input i, Log l, Structures.RawImage image, Guid currentGuid)
        {
            this._m = m;
            this._i = i;
            this._l = l;
            this._image = image;
            this._runGuid = currentGuid;
        }

        public void Start()
        {
            _l.Event(_runGuid, "Started Multi Threaded Canny Edge Detection");
            bool confirmOptions = false;
            CannyEdgeDetection detector;

            _l.Event(_runGuid, "Getting Multi Thread Options");

            do
            {
                detector = GetDetector(_m, _i, _l);

                string opt = _i.GetInput("Are you happy with those edge detection variables (y/n): ");
                if (opt.ToLower() == "y") confirmOptions = true;
                else _m.ClearUserSection();
            } while (!confirmOptions);


            Structures.RGB[][,] quads = Utility.SplitImage(_image.Pixels);
            Task<double[,]>[] threads = new Task<double[,]>[quads.Length];

            int continueOption = _i.GetOption("Continue to Canny Edge Detection:", new[] { "Yes - Continue", "No - Return to main menu" });
            if (continueOption != 0) throw new Exception("Map Processing Ended At User Request");

            bool saveTempOption = _i.GetOption("Would you like to save images at each step of the edge detection?", new[] { "Yes", "No" }) == 0;

            ProgressBar pb = new ProgressBar("Canny Edge Detection", 36, _m);
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
            double[,] cannyImage = Utility.CombineQuadrants(threads[0].Result, threads[1].Result, threads[2].Result,
                threads[3].Result);

            PostProcessImage(cannyImage);
        }

        private void PostProcessImage(double[,] image)
        {
            Post postProcessor = new Post(image);

            _m.ClearUserSection();
            if (_i.TryGetInt("How many times would you like to emboss the image (can be 0): ", out int loopCount) &&
                loopCount > 0)
            {
                _m.WriteLine();
                _m.WriteLine($"Running image embossing this will take approximately {Log.Red}{10*loopCount}\x1b[0m seconds!");
                postProcessor.Start(loopCount);
            }
            else
            {
                _m.WriteLine();
                _m.WriteLine($"Running image embossing this will take approximately {Log.Red}10\x1b[0m seconds!");
                postProcessor.Start(0);
            }

            _resultArray = postProcessor.Result();
        }

        private double[,] RunDetectionOnQuadrant(CannyEdgeDetection detector, Structures.RGB[,] image, int id, Action increment, bool saveTemp)
        {
            char letter = (char)('A' + id);
            double[,] workingArray;
            _l.Event(_runGuid, $"Starting processing of quadrant {letter} ({id % 2}, {id / 2})");

            workingArray = detector.BlackWhiteFilter(image);
            if (saveTemp) Logger.SaveBitmap(_runGuid, workingArray, $"BlackWhiteFilterQuad{letter}");
            increment();
            _l.Event(_runGuid, $"Completed Black and White Filter on Quadrant {letter}");

            workingArray = detector.GaussianFilter(workingArray);
            if (saveTemp) Logger.SaveBitmap(_runGuid, workingArray, $"GaussianFilterQuad{letter}");
            increment();
            _l.Event(_runGuid, $"Applied Gaussian Filter on Quadrant {letter}");

            Structures.Gradients grads = detector.CalculateGradients(workingArray, increment);
            if (saveTemp)
            {
                Logger.SaveBitmap(_runGuid, grads.GradientX, $"GradientXQuad{letter}");
                Logger.SaveBitmap(_runGuid, grads.GradientY, $"GradientYQuad{letter}");
            }
            _l.Event(_runGuid, $"Calculated Gradients for Quadrant {letter}");

            double[,] combinedGrads = detector.CombineGradients(grads);
            if (saveTemp) Logger.SaveBitmap(_runGuid, combinedGrads, $"CombinedGradientsQuad{letter}");
            increment();
            _l.Event(_runGuid, $"Calculated Combined Gradients for Quadrant {letter}");

            double[,] angleGrads = detector.GradientAngle(grads);
            increment();
            // Convert to readable?
            if (saveTemp)
            {
                for (int y = 0; y < angleGrads.GetLength(0); y++)
                for (int x = 0; x < angleGrads.GetLength(1); x++)
                    workingArray[y, x] = Utility.MapRadiansToPixel(angleGrads[y, x]);

                Logger.SaveBitmap(_runGuid, workingArray, $"AngleGradientsQuad{letter}");
            }
            _l.Event(_runGuid, $"Calculated Gradient Angles for Quadrant {letter}");

            workingArray = detector.MagnitudeThreshold(combinedGrads, angleGrads);
            if (saveTemp) Logger.SaveBitmap(_runGuid, workingArray, $"MagnitudeThresholdQuad{letter}");
            increment();
            _l.Event(_runGuid, $"Applied Magnitude Threshold on Quadrant {letter}");

            Structures.ThresholdPixel[,] thresholdArray = detector.DoubleThreshold(workingArray);
            increment();
            if (saveTemp)
            {
                Bitmap toSave = new Bitmap(thresholdArray.GetLength(1), thresholdArray.GetLength(0));
                for (int y = 0; y < thresholdArray.GetLength(0); y++)
                {
                    for (int x = 0; x < thresholdArray.GetLength(1); x++)
                    {
                        if (thresholdArray[y,x].Strong) toSave.SetPixel(x,y, Color.Green);
                        else if (!thresholdArray[y,x].Strong && thresholdArray[y,x].Value != 0) toSave.SetPixel(x,y, Color.Red);
                        else toSave.SetPixel(x,y, Color.Black);
                    }
                }
                Logger.SaveBitmap(_runGuid, toSave, $"ThresholdPixelsQuad{letter}");
            };
            // TODO just be same think about that colour strong weak etc...?
            _l.Event(_runGuid, $"Calculated Threshold Pixels for Quadrant {letter}");

            workingArray = detector.EdgeTrackingHysteresis(thresholdArray);
            if (saveTemp) Logger.SaveBitmap(_runGuid, workingArray, $"EdgeTrackingHysteresisQuad{letter}");
            increment();
            _l.Event(_runGuid, $"Applied Edge Tracking by Hysteresis on Quadrant {letter}");

            return workingArray;
        }

        private CannyEdgeDetection GetDetector(Menu m, Input i, Log l)
        {
            CannyEdgeDetection cannyDetection = new CannyEdgeDetection();

            if (i.TryGetDouble(
                        $"Enter a value for the ratio value for red for the Black and White filter (Default: {cannyDetection.RedRatio}, Range: 0 <= x <= 1)",
                    out double newRedRatio) && newRedRatio <= 1 && newRedRatio >= 0 && newRedRatio != cannyDetection.RedRatio)
            {
                l.Warn(_runGuid, $"Changed red ratio {cannyDetection.RedRatio} -> {newRedRatio}");
                m.WriteLine($"{Log.Orange}Changed: {cannyDetection.RedRatio} -> {newRedRatio}\x1b[0m");
                cannyDetection.RedRatio = newRedRatio;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.RedRatio}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the ratio value for green for the Black and White filter (Default: {cannyDetection.GreenRatio}, Range: 0 <= x <= 1)",
                    out double newGreenRatio) && newGreenRatio <= 1 && newGreenRatio >= 0 &&
                newGreenRatio != cannyDetection.GreenRatio)
            {
                l.Warn(_runGuid, $"Changed green ratio {cannyDetection.GreenRatio} -> {newGreenRatio}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.GreenRatio} -> {newGreenRatio}\x1b[0m");
                cannyDetection.GreenRatio = newGreenRatio;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.GreenRatio}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the ratio value for blue for the Black and White filter (Default: {cannyDetection.BlueRatio}, Range: 0 <= x <= 1)",
                    out double newBlueRatio) && newBlueRatio <= 1 && newBlueRatio >= 0 && newBlueRatio != cannyDetection.BlueRatio)
            {
                l.Warn(_runGuid, $"Changed blue ratio {cannyDetection.BlueRatio} -> {newBlueRatio}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.BlueRatio} -> {newBlueRatio}\x1b[0m");
                cannyDetection.BlueRatio = newBlueRatio;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.BlueRatio}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for sigma for the Gaussian Filter stage (Default: {cannyDetection.Sigma}, Range: 0 < x <= 10)",
                    out double newSigma) && newSigma <= 10 && newSigma > 0 && newSigma != cannyDetection.Sigma)
            {
                l.Warn(_runGuid, $"Changed sigma value {cannyDetection.Sigma} -> {newSigma}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.Sigma} -> {newSigma}\x1b[0m");
                cannyDetection.Sigma = newSigma;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.Sigma}\x1b[0m");
            m.WriteLine();

            if (i.TryGetInt(
                    $"Enter a value for kernel size for the Gaussian Filter stage, large values will take exponentially longer (Default: {cannyDetection.KernelSize}, Range: x >= 3, x not a multiple of 2 and a whole number)",
                    out int newKernel) && newKernel >= 3 && newKernel % 2 == 1 && newKernel % 1 == 0 && newKernel != cannyDetection.KernelSize)
            {
                l.Warn(_runGuid, $"Changed kernel size {cannyDetection.KernelSize} -> {newKernel}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.KernelSize} -> {newKernel}\x1b[0m");
                cannyDetection.KernelSize = newKernel;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.KernelSize}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the lower threshold for the Min Max stage (Default: {cannyDetection.LowerThreshold}, Range: 0 <= x < 1)",
                    out double newLowerThreshold) && newLowerThreshold > 0 && newLowerThreshold < 1 && newLowerThreshold != cannyDetection.LowerThreshold)
            {
                l.Warn(_runGuid, $"Changed lower threshold {cannyDetection.LowerThreshold} -> {newLowerThreshold}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.LowerThreshold} -> {newLowerThreshold}\x1b[0m");
                cannyDetection.LowerThreshold = newLowerThreshold;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.LowerThreshold}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the lower threshold for the Min Max stage (Default: {cannyDetection.UpperThreshold}, Range: {cannyDetection.LowerThreshold} < x <= 1)",
                    out double newHigherThreshold) && newHigherThreshold > cannyDetection.LowerThreshold && newHigherThreshold <= 1 && newHigherThreshold != cannyDetection.UpperThreshold)
            {
                l.Warn(_runGuid, $"Changed upper threshold {cannyDetection.UpperThreshold} -> {newHigherThreshold}");
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.UpperThreshold} -> {newHigherThreshold}\x1b[0m");
                cannyDetection.UpperThreshold = newHigherThreshold;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.UpperThreshold}\x1b[0m");
            m.WriteLine();

            i.GetInput("(Press enter to continue)");
            m.ClearUserSection();

            m.WriteLine("For reference the variables which will be used are:");
            m.WriteLine($"    Red Ratio: {Log.Orange}{cannyDetection.RedRatio}\x1b[0m");
            m.WriteLine($"    Green Ratio: {Log.Orange}{cannyDetection.GreenRatio}\x1b[0m");
            m.WriteLine($"    Blue Ratio: {Log.Orange}{cannyDetection.BlueRatio}\x1b[0m");
            m.WriteLine($"    Gaussian Sigma Value: {Log.Orange}{cannyDetection.Sigma}\x1b[0m");
            m.WriteLine($"    Gaussian Kernel Size: {Log.Orange}{cannyDetection.KernelSize}\x1b[0m");
            m.WriteLine($"    Double Threshold Lower: {Log.Orange}{cannyDetection.LowerThreshold}\x1b[0m");
            m.WriteLine($"    Double Threshold Upper: {Log.Orange}{cannyDetection.UpperThreshold}\x1b[0m");
            m.WriteLine();

            return cannyDetection;
        }

        public double[,] Result() => _resultArray;
    }
}
