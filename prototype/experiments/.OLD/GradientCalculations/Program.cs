using System;
using System.ComponentModel.Design;
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
            Bitmap image = new Bitmap("image_blur.jpg");
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

            gradY.Save("image_gradY.jpg");
            gradX.Save("image_gradX.jpg");
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


                    // note to self: direciton doesnt matter so we can take the modulo of the theta value
                    // Note that the sign of the direction is irrelevant, i.e. north–south is the same as south–north and so on.
                    double theta = Math.Abs(180 * Math.Atan2(gradYArray[i, j], gradXArray[i, j]) / Math.PI);
                    thetaArray[i, j] = theta;
                }
            }

            bigGImage.Save("image_combinedGradients.jpg");

            double[,] magnitudeThresholding = bigGArray;

            // begin edge thining
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    double[,] kernelMatrix = BuildKernel(j, i, bigGArray)._matrix;

                    if (thetaArray[i, j] < 22.5 || thetaArray[i, j] >= 157.5)
                    {
                        if (bigGArray[i, j] < kernelMatrix[1, 2] || bigGArray[i, j] < kernelMatrix[1, 0])
                        {
                            magnitudeThresholding[i, j] = 0;
                        }
                    }
                    else if (thetaArray[i, j] >= 22.5 && thetaArray[i, j] < 67.5)
                    {
                        if (bigGArray[i, j] < kernelMatrix[0, 2] || bigGArray[i, j] < kernelMatrix[2, 0])
                        {
                            magnitudeThresholding[i, j] = 0;
                        }
                    }
                    else if (thetaArray[i, j] >= 67.5 && thetaArray[i, j] < 112.5)
                    {
                        if (bigGArray[i, j] < kernelMatrix[0, 1] || bigGArray[i, j] < kernelMatrix[2, 1])
                        {
                            magnitudeThresholding[i, j] = 0;
                        }
                    }
                    else if (thetaArray[i, j] >= 112.5 && thetaArray[i, j] < 157.5)
                    {
                        if (bigGArray[i, j] < kernelMatrix[0, 0] || bigGArray[i, j] < kernelMatrix[2, 2])
                        {
                            magnitudeThresholding[i, j] = 0;
                        }
                    }
                    else throw new Exception("This shouldn't happen but i want to know if it does");
                }
            }

            Bitmap magnitudeThresholdImage = new Bitmap(image.Width, image.Height);

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    int value = (int)(magnitudeThresholding[i,j] > 0 ? (magnitudeThresholding[i,j] > 255 ? 255 : magnitudeThresholding[i, j]) : 0);
                    magnitudeThresholdImage.SetPixel(j, i, Color.FromArgb(255, value, value, value));
                }
            }

            magnitudeThresholdImage.Save("image_magnitudeThreasholding.jpg");
            magnitudeThresholdImage.Dispose();

            // max min grad trimming
            double high = 100;
            double low = 50;

            Bitmap doubleMaxMinImage = new Bitmap(image.Width, image.Height);
            (double, bool)[,] finalBeforeHysteresis = new (double, bool)[image.Height, image.Width];
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    if (thetaArray[i, j] < low)
                    {
                        finalBeforeHysteresis[i, j] = (0, false);
                        doubleMaxMinImage.SetPixel(j, i, Color.Black);
                    } else if (thetaArray[i, j] >= low && thetaArray[i, j] < high)
                    {
                        finalBeforeHysteresis[i, j] = (magnitudeThresholding[i, j], false);
                        int value = (int)(magnitudeThresholding[i, j] > 0 ? (magnitudeThresholding[i, j] > 255 ? 255 : magnitudeThresholding[i, j]) : 0);
                        doubleMaxMinImage.SetPixel(j, i, Color.FromArgb(255, value, value, value));
                    } else if (thetaArray[i, j] >= high)
                    {
                        finalBeforeHysteresis[i, j] = (magnitudeThresholding[i, j], true);
                        int value = (int)(magnitudeThresholding[i, j] > 0 ? (magnitudeThresholding[i, j] > 255 ? 255 : magnitudeThresholding[i, j]) : 0);
                        doubleMaxMinImage.SetPixel(j, i, Color.FromArgb(255, value, value, value));
                    }
                    else throw new Exception("This shouldn't happen but i want to know if it does");
                }   
            }

            doubleMaxMinImage.Save("iamge_maxMinThresholding.jpg");

            // hysteresis time baby
            Bitmap finalImage = new Bitmap(image.Width, image.Height);

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    if (finalBeforeHysteresis[i, j].Item2 == false)
                    {
                        (double, bool)[,] blob = BuildKernel(j, i, finalBeforeHysteresis);

                        bool keeper = false;

                        // begin search for the last true blob
                        for (int k = 0; k < blob.GetLength(1); k++)
                        {
                            for (int l = 0; l < blob.GetLength(0); l++)
                            {
                                if (blob[k, l].Item2) keeper = true;
                            }
                        }

                        if (keeper) finalImage.SetPixel(j, i, Color.White);
                    }
                    else
                    {
                        finalImage.SetPixel(j, i, Color.White);
                    }
                }
            }

            finalImage.Save("iamge_final.jpg");
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

        public static (double, bool)[,] BuildKernel(int x, int y, (double, bool)[,] image)
        {
            (double, bool)[,] kernel = new (double, bool)[3, 3];

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

            return kernel;
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
