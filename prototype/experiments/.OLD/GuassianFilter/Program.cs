using System.Drawing;

namespace GuassianFilter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("image.jpg");

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
            blackWhite.Save("image_bw.jpg");
            blackWhite.Dispose();


            double sigma = 1.4;
            int k = 5;
            double kSum = 159;

            double[,] kernel = new double[,]
            {
                {2, 4, 5, 4, 2},
                {4, 9, 12, 9, 4},
                {5, 12, 15, 12, 5},
                {4, 9, 12, 9, 4},
                {2, 4, 5, 4, 2}
            };
            //for (int i = 0; i < k; i++)
            //{
            //    for (int j = 0; j < k; j++)
            //    {
            //        kernel[i, j] = 1 / (2 * Math.PI * sigma * sigma) *
            //            Math.Exp(-((Math.Pow(i, 2) + Math.Pow(j, 2)) / (2 * sigma * sigma)));
            //        kSum += kernel[i, j];
            //    }
            //}

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
                    Matrix kernelMatrix = new Matrix(kernel);
                    Matrix imageSection = BuildKernel(j, i, bwImage);
                    double sum = Matrix.Convolution(kernelMatrix, imageSection);

                    bluredImage.SetPixel(j, i, Color.FromArgb(255, (int)sum, (int)sum, (int)sum));
                }
            }

            bluredImage.Save("image_blur.jpg");
        }

        public static Matrix BuildKernel(int x, int y, double[,] image)
        {
            double[,] kernel = new double[5, 5];

            // prefill incase of edge
            for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) kernel[i, j] = image[y, x];

            int cntY = 0;
            for (int j = y - 1; j <= y + 1; j++)
            {
                int cntX = 0;
                for (int i = x - 1; i <= x + 1; i++)
                {
                    if (j >= 0 && i >= 0 && j < image.GetLength(0) && i < image.GetLength(1))
                    {
                        kernel[cntY, cntX] = image[j, i];
                    }
                    cntX++;
                }
                cntY++;
            }

            return new Matrix(kernel);
        }


    }






    public class Matrix
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public double[,] _matrix { get; private set; }

        public Matrix(double[,] input)
        {
            _matrix = input;
            x = input.GetLength(1);
            y = input.GetLength(0);
        }

        public static double Convolution(Matrix a, Matrix b)
        {
            double[,] flippedB = new double[b.y, b.x];
            for (int i = b.y - 1; i >= 0; i--)
            {
                for (int j = b.x - 1; j >= 0; j--)
                {
                    flippedB[b.y - (i + 1), b.x - (j + 1)] = b._matrix[i, j];
                }
            }

            double sum = 0;
            for (int i = 0; i < a.y; i++)
            {
                for (int j = 0; j < a.x; j++)
                {
                    sum += a._matrix[i, j] * flippedB[i, j];
                }
            }

            return sum;
        }


    }
}
