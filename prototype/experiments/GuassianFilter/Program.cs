using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;

namespace GuassianFilter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("flower.jpg");

            double[,] bwImage = new double[image.Height, image.Width];
            for (int y = 0; y < image.Height; y++) for (int x = 0; x < image.Width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                double val = pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114;
                if (val > 255) val = 255;
                if (val < 0) val = 0;
                bwImage[y, x] = val;
            }

            Bitmap blackWhite = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < bwImage.GetLength(0); y++)
            for (int x = 0; x < bwImage.GetLength(1); x++)
            {
                blackWhite.SetPixel(x, y, Color.FromArgb(255, (int)bwImage[y, x], (int)bwImage[y, x], (int)bwImage[y, x]));
            }
            blackWhite.Save("flower_bw.jpg");
            blackWhite.Dispose();


            double sigma = 4;
            int k = 5;
            double kSum = 0;

            double[,] kernel = new double[k, k];
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    kernel[i, j] = 1 / (2 * Math.PI * sigma * sigma) *
                        Math.Exp(-((Math.Pow(i, 2) + Math.Pow(j, 2)) / (2 * sigma * sigma)));
                    kSum += kernel[i, j];
                }
            }

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    kernel[i, j] /= kSum;
                }
            }


            Bitmap bluredImage = new Bitmap(image.Width, image.Height);

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {

                    double[,] imageSection = BuildKernel(j, i, bwImage);
                    double sum = 0;

                    for (int y = 0; y < imageSection.GetLength(1); y++)
                    {
                        for (int x = 0; x < imageSection.GetLength(0); x++)
                        {
                            sum += imageSection[y, x] * kernel[y, x];
                        }
                    }

                    bluredImage.SetPixel(j, i, Color.FromArgb(255, (int)sum, (int)sum, (int)sum));
                }
            }
            
            bluredImage.Save("flower_blur.jpg");
        }

        public static double[,] BuildKernel(int x, int y, double[,] image)
        {
            double[,] kernel = new double[5, 5];
            
            // prefill incase of edge
            for (int i = 0; i < 5; i++) for (int j = 0; j < 5; j++) kernel[i, j] = image[y, x];

            int cntY = 0;
            for (int j = y - 2; j <= y + 2; j++)
            {
                int cntX = 0;
                for (int i = x - 2; i <= x + 2; i++)
                {
                    if (j >= 0 && i >= 0 && j < image.GetLength(0) && i < image.GetLength(1))
                    {
                        kernel[cntY, cntX] = image[j, i];
                    }
                    cntX++;
                }
                cntY++;
            }

            return kernel;
        }
    }
}
