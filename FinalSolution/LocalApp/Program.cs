using BackendLib;
using BackendLib.Data;
using BackendLib.Processing;
using LocalApp.CLI;
using System;
using System.IO;

namespace LocalApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Menu menu = new Menu("Author: Rubens Pirie", "\x1b[38;5;119mDevelopment Mode\x1b[0m");
            Input inputs = new Input(menu);
            Log logger = new Log(menu);

            menu.Setup();
            logger.Event("Program has started and menu has been created successfully.");

            Run(menu, inputs, logger);
        }

        private static void Run(Menu m, Input i, Log l)
        {
            bool running = true;

            while (running)
            {
                int opt = i.GetOption("Please select an option to continue:",
                    new[]
                    {
                        "Process New Image Into Map Data File", "Recall Map From Data File", "Exit Program", "Dev Test"
                    });

                switch (opt)
                {
                    // New
                    case 0:
                        RunNewImage(m, i, l);

                        break;
                    // Recall
                    case 1:
                        Map recalledMap = new Map();

                        break;
                    case 2:
                        running = false;
                        break;
                    case 3:
                        DevTest(m, i, l);
                        break;
                }
            }
        }

        private static void RunNewImage(Menu m, Input i, Log l)
        {
            Guid runGuid = Logger.CreateRun();
            l.Event(runGuid, $"Begin processing of new image (Run Id: {runGuid}).");

            NewImage newImage = new NewImage(m, i, l, runGuid);

            try
            {
                Structures.RawImage rawImage = newImage.Read();

                int opt = i.GetOption("Select a version of edge detection to run:",
                    new[]
                    {
                        "Multi-threaded - Fast, all options decided at the start",
                        "Synchronous - Slow, options can be changed after each step and steps can be repeated"
                    });

                double[,] cannyImage = opt == 0 ? AsyncEdgeDetection(m, i, l, rawImage) : SyncEdgeDetection(m, i, l, rawImage);

                // TODO Next section move onto graph stuff

                l.EndSuccessRun(runGuid);
            }
            catch (Exception ex)
            {
                l.EndErrorRun(runGuid, ex);
            }
        }

        // TODO: Need to somehow split some of the following into sep classes

        private static CannyEdgeDetection GetAsyncDetector(Menu m, Input i)
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

        private static double[,] AsyncEdgeDetection(Menu m, Input i, Log l, Structures.RawImage image)
        {
            bool confirmOptions = false;
            CannyEdgeDetection detector;
            do
            {
                detector = GetAsyncDetector(m, i);

                int opt = i.GetOption("Are you happy with those edge detection variables",
                    new[] { "Yes: Proceed to Canny Edge Detection", "No: Re-enter all variables." });
                if (opt == 0) confirmOptions = true;
            } while (!confirmOptions);

            detector.BlackWhiteFilter(image.Pixels);

            return new double[0, 0];
        }

        private static double[,] SyncEdgeDetection(Menu m, Input i, Log l, Structures.RawImage image)
        {
            throw new NotImplementedException();
            return new double[0, 0];
        }

        private static void DevTest(Menu m, Input i, Log l)
        {
            int opt = i.GetOption("Dev Test Options",
                new[] { "Wipe logs and run files including all saves", "Run automated demo", "N/A", "N/A" });

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


                    break;
                case 2:


                    break;


                case 3:


                    break;
            }
        }
    }
}