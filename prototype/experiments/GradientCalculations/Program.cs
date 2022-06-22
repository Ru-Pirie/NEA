using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GradientCalculations
{
    internal class Program
    {
        // step 2
        // https://en.wikipedia.org/wiki/Sobel_operator
        // https://en.wikipedia.org/wiki/Canny_edge_detector#Finding_the_intensity_gradient_of_the_image
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("image.jpg");
            double[,] imageGrid = new double[image.Height, image.Width];

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    imageGrid[i, j] = image.GetPixel(j, i).R;
                }
            }

            Bitmap gradX = new Bitmap(image.Width, image.Height);
            Bitmap gradY = new Bitmap(image.Width, image.Height);

            Matrix gradXMatrix = new Matrix(new double[,] { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } });
            Matrix gradYMatrix = new Matrix(new double[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } });

            double[,] gradXArray = new double[image.Height, image.Width];
            double[,] gradYArray = new double[image.Height, image.Width];

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Matrix kernel = BuildKernel(j, i, imageGrid);
                    double xConvolution = Matrix.Convolution(gradXMatrix, kernel);
                    double yConvolution = Matrix.Convolution(gradYMatrix, kernel);

                    gradXArray[i, j] = xConvolution;
                    gradYArray[i, j] = yConvolution;


                    // only needed for image
                    xConvolution = xConvolution > 0 ? (xConvolution > 255 ? 255 : xConvolution) : 0;
                    yConvolution = yConvolution > 0 ? (yConvolution > 255 ? 255 : yConvolution) : 0;
                    
                    gradX.SetPixel(j, i, Color.FromArgb(255, (int)xConvolution, (int)xConvolution, (int)xConvolution));
                    gradY.SetPixel(j, i, Color.FromArgb(255, (int)yConvolution, (int)yConvolution, (int)yConvolution));
                }
            }

            gradY.Save("gradY.jpg");
            gradX.Save("gradX.jpg");
            gradY.Dispose();
            gradX.Dispose();

            Bitmap bigGImage = new Bitmap(image.Width, image.Height);
            double[,] thetaArray = new double[image.Height, image.Width];
            double[,] bigGArray = new double[image.Height, image.Width];


            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    double G = Math.Sqrt(Math.Pow(gradXArray[i, j], 2) + Math.Pow(gradYArray[i, j], 2));
                    bigGArray[i, j] = G;

                    // only for the image
                    G = G > 0 ? (G > 255 ? 255 : G) : 0;
                    bigGImage.SetPixel(j, i, Color.FromArgb(255, (int)G, (int)G, (int)G));
                    

                    double theta = Math.Atan2(gradYArray[i, j], gradXArray[i, j]);
                    thetaArray[i, j] = theta;
                }
            }

            bigGImage.Save("bigG.jpg");
            double max = thetaArray.Cast<double>().Max();
            double min = thetaArray.Cast<double>().Min();
            // begin edge thining
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Matrix kernelMatrix = BuildKernel(j, i, bigGArray);

                    if (thetaArray[i, j] > 0 && thetaArray[i, j] < 45)
                    {

                    }
                }
            }
        }

        public static Matrix BuildKernel(int x, int y, double[,] image)
        {
            double[,] kernel = new double[3, 3];

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
