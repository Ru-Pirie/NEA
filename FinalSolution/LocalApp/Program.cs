using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackendLib;
using BackendLib.Data;
using BackendLib.Datatypes;
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

            //menu.Setup();
            //logger.Event("Program has started and menu has been created successfully.");

            Pre preImage = new Pre("image.jpg");
            preImage.Start();
            CannyEdgeDetection detector = new CannyEdgeDetection();

            double[,] a = detector.BlackWhiteFilter(preImage.Result().Pixels);
            double[,] b = detector.GaussianFilter(a);
            Structures.Gradients c = detector.CalculateGradients(b, thing);

            double[,] d = detector.CombineGradients(c);
            double[,] e = detector.GradientAngle(c);

            double[,] f = detector.MagnitudeThreshold(d, e);
            Structures.ThresholdPixel[,] g = detector.DoubleThreshold(f);
            double[,] q = detector.EdgeTrackingHysteresis(g);

            Post postImage = new Post(q);
            postImage.Start(0);

            postImage.Result().ToBitmap().Save("out.png");
            RoadDetection roadDetector = new RoadDetection(postImage.Result(), 0.25);
            roadDetector.Start(thing);
            roadDetector.Result().PathBitmap.Save("path.png");

            Bitmap combined = Utility.CombineBitmap(preImage.Result().Original, roadDetector.Result().PathBitmap);

            combined.Save("combinedFinal.png");

            Bitmap img = roadDetector.Result().PathBitmap;

            double[,] temp = new double[img.Height, img.Width];
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    temp[y, x] = pixel.R + pixel.G + pixel.B != 0 ? 255 : 0;
                }
            }
            temp.ToBitmap().Save("white.png");
            Graph<Structures.Cord> myGraph = temp.ToGraph();
            Traversal<Structures.Cord> traversalObj = new Traversal<Structures.Cord>(myGraph);
            Structures.Cord[] DSFARR = traversalObj.DFS(new Structures.Cord { X = 249, Y = 0 });

            Bitmap checkBitmap = new Bitmap(temp.ToBitmap());
            //for (int i = 0; i < 768; i++)
            //{
            //    int B = i < 256 ? i : 255;
            //    int R = (i < 512 ? (i > 256 ? i - 256 : 0) : 255);
            //    int G = (i < 768 ? (i > 512 ? i - 512 : 0) : 255);
            //    checkBitmap.SetPixel(DSFARR[i].X, DSFARR[i].Y, Color.FromArgb(R, G, B));
            //}

            int i = 0;
            foreach (var d1 in DSFARR)
            {
                checkBitmap.SetPixel(d1.X, d1.Y, Color.BlueViolet);
                i++;
                if (i % 4 == 0)checkBitmap.Save($"./saves/{i / 4}.png");
            }
            checkBitmap.Save($"./saves/{++i}.png");

            checkBitmap.Save("CheckBitmap.png");

            var aC = new Structures.Cord { X = 1, Y = 1 };
            var bC = new Structures.Cord { X = 1, Y = 1 };

            Console.WriteLine(aC.Equals(bC));

            Console.ReadLine();
            Environment.Exit(0);

            menu.WriteLine("DONE");

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
