using System;
using System.ComponentModel;
using System.Drawing;

namespace ExperimentalFilling
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("image.jpg");

            double[,] imageArray = new double[image.Height, image.Width];
            for (int y = 0; y < image.Height; y++) for (int x = 0; x < image.Width; x++) imageArray[y, x] = image.GetPixel(x, y).R;

            double[,] tempArray = PadImage(imageArray);
            DoubleArrayToBitmap(tempArray).Save("filledImage.jpg");
        }

        public static double[,] PadImage(double[,] image)
        {
            double[,] result = image;

            for (int i = 1; i < image.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < image.GetLength(1) - 1; j++)
                {
                    double[,] kernel = BuildKernel(j, i, 3, image);
                    int sum = 0;
                    foreach (double value in kernel) if (value >= 255) sum++;
                    if (sum >= 5)
                    {
                        for (int k = i - 1; k < i + 1; k++)
                        {
                            for (int l = j - 1; l < j + 1; l++)
                            {
                                result[k, l] = 255;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static int Bound(int l, int h, double v) => v > h ? h : (v < l ? l : (int)v);

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

        public static double[,] BuildKernel(int x, int y, int k, double[,] grid)
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

            return kernel;
        }
    }
}
