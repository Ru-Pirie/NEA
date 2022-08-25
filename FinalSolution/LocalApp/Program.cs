using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackendLib;
using BackendLib.Data;
using BackendLib.Processing;
using LocalApp.CLI;

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

            Pre preImage = new Pre("image.jpg");
            preImage.Start();
            CannyEdgeDetection detector = new CannyEdgeDetection();

            detector.KernelSize = 3;

            double[,] a = detector.BlackWhiteFilter(preImage.Result().Pixels);
            double[,] b = detector.GaussianFilter(a);
            Structures.Gradients c = detector.CalculateGradients(b, thing);

            double[,] d = detector.CombineGradients(c);
            double[,] e = detector.GradientAngle(c);

            double[,] f =  detector.MagnitudeThreshold(d, e);
            Structures.ThresholdPixel[,] g = detector.DoubleThreshold(f);
            double[,] q = detector.EdgeTrackingHysteresis(g);

            Post postImage = new Post(q);
            postImage.Start(0);

            postImage.Result().ToBitmap().Save("out.png");
            RoadDetection roadDetector = new RoadDetection(postImage.Result(), 0.25);
            roadDetector.Start(thing);
            roadDetector.Result().PathBitmap.Save("path.png");

            Utility.CombineBitmap(preImage.Result().Original, roadDetector.Result().PathBitmap).Save("combinedFinal.png");

            Run(menu, inputs, logger);
        }

        private static void thing() => Console.Title = "thing";

        private static void Run(Menu m, Input i, Log l)
        {
            bool running = true;

            while (running)
            {
                int opt = i.GetOption("Please select an option to continue:",
                    new[] { "Process New Image Into Map Data File", "Recall Map From Data File", "Exit Program" });

                switch (opt)
                {
                    case 0: 



                        break;
                    case 1:



                        break;
                    case 2:
                        running = false;
                        break;
                }
            }
        }

    }
}
