using System;
using System.Drawing;
using BackendLib.Processing;

namespace BackendLib
{
    public static class Utility
    {
        public static double GaussianDistribution(int x, int y, double sigma) =>
            1 / (2 * Math.PI * sigma * sigma) * Math.Exp(-((Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * sigma * sigma)));

        public static double Bound(int l, int h, double v) => v > h ? h : v < l ? l : v;

        public static bool TryBound(int l, int h, double v, out double value)
        {
            if (v < h && v > l) value = v;
            else value = v > h ? h : l;
            return v < h && v > l;
        }

        public static double RadianToDegree(double input) => 180 * input / Math.PI;

        public static double DegreeToRadian(double input) => input * Math.PI / 180;

        public static double MapRadiansToPixel(double input) => (int)(128 / (2 * Math.PI) * input + 128);

        public static Bitmap CombineBitmap(Bitmap a, Bitmap b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
                throw new ArgumentException("Supplied Bitmaps Where Not Of Same Dimensions");

            Bitmap result = a;
            for (int y = 0; y < a.Height; y++)
            {
                for (int x = 0; x < a.Width; x++)
                {
                    Color pixel = b.GetPixel(x, y);
                    if (pixel != Color.FromArgb(0, 0, 0))
                    {
                        result.SetPixel(x, y, pixel);
                    }
                }
            }

            return result;
        }

        // TODO: Compress to one for loop, tried before ended badly
        public static Bitmap[] SplitImage(Structures.RGB[,] image)
        {
            Structures.RGB[,] one = new Structures.RGB[,](image.GetLength(1) / 2, image.GetLength(0) / 2);
            Structures.RGB[,] two = new Structures.RGB[,](image.GetLength(1) / 2, image.GetLength(0) / 2);
            Structures.RGB[,] three = new Structures.RGB[,](image.GetLength(1) / 2, image.GetLength(0) / 2);
            Structures.RGB[,] four = new Structures.RGB[,](image.GetLength(1) / 2, image.GetLength(0) / 2);

            for (int i = 0; i < image.GetLength(1) / 2; i++)
            {
                for (int j = 0; j < image.GetLength(0) / 2; j++)
                {
                    one.SetPixel(i, j, image.GetPixel(i, j));
                }
            }

            for (int i = image.GetLength(1) / 2; i < image.GetLength(1); i++)
            {
                for (int j = 0; j < image.GetLength(0) / 2; j++)
                {
                    two.SetPixel(i - (image.GetLength(1) / 2), j, image.GetPixel(i, j));
                }
            }

            for (int i = 0; i < image.GetLength(1) / 2; i++)
            {
                for (int j = image.GetLength(0) / 2; j < image.GetLength(0); j++)
                {
                    three.SetPixel(i, j - (image.GetLength(0) / 2), image.GetPixel(i, j));
                }
            }

            for (int i = image.GetLength(1) / 2; i < image.GetLength(1); i++)
            {
                for (int j = image.GetLength(0) / 2; j < image.GetLength(0); j++)
                {
                    four.SetPixel(i - (image.GetLength(1) / 2), j - (image.GetLength(0) / 2), image.GetPixel(i, j));
                }
            }

            return new[] { one, two, three, four };

        }

        public static bool VerifyCannyEdgeDetectionOptions(CannyEdgeDetection classObject)
        {
            // TODO: Add checks for class
            // if (opts.KernelSize % 2 != 1) return false;
            // if (opts.UpperThreshold < opts.LowerThreshold) return false;
            // if (opts.UpperThreshold <= 0) return false;
            // if (opts.LowerThreshold >= 1) return false;
            // if (opts.BlueRatio + opts.RedRatio + opts.GreenRatio > 1) return false;

            return true;
        }
    }
}
