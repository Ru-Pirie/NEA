using BackendLib;
using BackendLib.Processing;
using LocalApp.CLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendLib.Interfaces;

namespace LocalApp
{
    internal class AsyncEdgeDetection : IHandler
    {
        private Menu m;
        private Input i;
        private Log l;
        private Structures.RawImage image;
        private double[,] _workingArray;

        public AsyncEdgeDetection(Menu m, Input i, Log l, Structures.RawImage image)
        {
            this.m = m;
            this.i = i;
            this.l = l;
            this.image = image;
        }

        public void Start()
        {
            bool confirmOptions = false;
            CannyEdgeDetection detector;
            do
            {
                detector = GetDetector(m, i);

                string opt = i.GetInput("Are you happy with those edge detection variables (y/n): ");
                if (opt.ToLower() == "y") confirmOptions = true;
                else m.ClearUserSection();
            } while (!confirmOptions);


            Structures.RGB[][,] quads = Utility.SplitImage(image.Pixels);
            Task<double[,]>[] threads = new Task<double[,]>[quads.Length];

            // TODO: Add option to log indervidual steps?

            for (int i = 0; i < quads.Length; i++)
            {
                // Overcome Capture Condition
                int copyI = i;
                Task<double[,]> task = new Task<double[,]>(() => RunDetectionOnQuadrant(quads[i], i + 1));
                task.Start();
                threads[i] = task;
            }            
        }

        // TODO
        private double[,] RunDetectionOnQuadrant(Structures.RGB[,] image, int id)
        {
            throw new NotImplementedException();
        }

        private static CannyEdgeDetection GetDetector(Menu m, Input i)
        {
            CannyEdgeDetection cannyDetection = new CannyEdgeDetection();

            if (i.TryGetDouble(
                        $"Enter a value for the ratio value for red for the Black and White filter (Default: {cannyDetection.RedRatio}, Range: 0 <= x <= 1)",
                    out double newRedRatio) && newRedRatio <= 1 && newRedRatio >= 0 && newRedRatio != cannyDetection.RedRatio)
            {
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
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.GreenRatio} -> {newGreenRatio}\x1b[0m");
                cannyDetection.GreenRatio = newGreenRatio;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.GreenRatio}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the ratio value for blue for the Black and White filter (Default: {cannyDetection.BlueRatio}, Range: 0 <= x <= 1)",
                    out double newBlueRatio) && newBlueRatio <= 1 && newBlueRatio >= 0 && newBlueRatio != cannyDetection.BlueRatio)
            {
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.BlueRatio} -> {newBlueRatio}\x1b[0m");
                cannyDetection.BlueRatio = newBlueRatio;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.BlueRatio}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for sigma for the Gaussian Filter stage (Default: {cannyDetection.Sigma}, Range: 0 < x <= 10)",
                    out double newSigma) && newSigma <= 10 && newSigma > 0 && newSigma != cannyDetection.Sigma)
            {
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.Sigma} -> {newSigma}\x1b[0m");
                cannyDetection.Sigma = newSigma;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.Sigma}\x1b[0m");
            m.WriteLine();

            if (i.TryGetInt(
                    $"Enter a value for kernel size for the Gaussian Filter stage, large values will take exponentially longer (Default: {cannyDetection.KernelSize}, Range: x >= 3, x not a multiple of 2 and a whole number)",
                    out int newKernel) && newKernel >= 3 && newKernel % 2 == 1 && newKernel % 1 == 0 && newKernel != cannyDetection.KernelSize)
            {
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.KernelSize} -> {newKernel}\x1b[0m");
                cannyDetection.KernelSize = newKernel;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.KernelSize}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the lower threshold for the Min Max stage (Default: {cannyDetection.LowerThreshold}, Range: 0 <= x < 1)",
                    out double newLowerThreshold) && newLowerThreshold > 0 && newLowerThreshold < 1 && newLowerThreshold != cannyDetection.LowerThreshold)
            {
                m.WriteLine($"\x1b[38;5;2mChanged: {cannyDetection.LowerThreshold} -> {newLowerThreshold}\x1b[0m");
                cannyDetection.LowerThreshold = newLowerThreshold;
            }
            else m.WriteLine($"\x1b[38;5;3mKept Default: {cannyDetection.LowerThreshold}\x1b[0m");
            m.WriteLine();

            if (i.TryGetDouble(
                        $"Enter a value for the lower threshold for the Min Max stage (Default: {cannyDetection.UpperThreshold}, Range: {cannyDetection.LowerThreshold} < x <= 1)",
                    out double newHigherThreshold) && newHigherThreshold > cannyDetection.LowerThreshold && newHigherThreshold <= 1 && newHigherThreshold != cannyDetection.UpperThreshold)
            {
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


        public double[,] Result() => _workingArray;


    }
}
