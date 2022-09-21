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
