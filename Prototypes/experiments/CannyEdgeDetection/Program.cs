using System;
using System.Drawing;
using System.Threading.Tasks;

namespace CannyEdgeDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            var thing = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Beginning Edge Detection...");
            Bitmap input = new Bitmap("image.jpg");
            input.Save("./out/a.jpg");

            Console.WriteLine("1. Converting to Black and White");
            double[,] bwArray = BWFilter(input);
            Bitmap bwImage = DoubleArrayToBitmap(bwArray);
            bwImage.Save("./out/b.jpg");
            bwImage.Dispose();

            Console.WriteLine("2. Beginning Gaussian Filter");
            double[,] gaussianArray = GaussianFilter(1.4, 5, bwArray);
            Bitmap gaussianImage = DoubleArrayToBitmap(gaussianArray);
            gaussianImage.Save("./out/c.jpg");
            gaussianImage.Dispose();

            Console.WriteLine("3. Beginning Gradient Calculations");

            Task<double[,]>[] tasks = new Task<double[,]>[2];
            tasks[0] = new Task<double[,]>(() => CalculateGradientX(gaussianArray));
            tasks[1] = new Task<double[,]>(() => CalculateGradientY(gaussianArray));

            foreach (var task in tasks) task.Start();
            Task.WaitAll(tasks);

            Bitmap gradientXImage = DoubleArrayToBitmap(tasks[0].Result);
            Bitmap gradientYImage = DoubleArrayToBitmap(tasks[1].Result);
            gradientXImage.Save("./out/d.jpg");
            gradientYImage.Save("./out/e.jpg");
            gradientXImage.Dispose();
            gradientYImage.Dispose();

            Console.WriteLine("4. Beginning Total Gradient Calculations");
            double[,] gradientCombined = CalculateGradientCombined(tasks[0].Result, tasks[1].Result);
            Bitmap gradientCombinedImage = DoubleArrayToBitmap(gradientCombined);
            gradientCombinedImage.Save("./out/f.jpg");
            gradientCombinedImage.Dispose();

            Console.WriteLine("5. Calculating Gradient Angles Calculations");
            double[,] thetaArray = CalculateTheta(tasks[0].Result, tasks[1].Result);
            Bitmap thetaImage = ConvertThetaToBitmap(thetaArray);
            thetaImage.Save("./out/g.jpg");
            thetaImage.Dispose();

            Console.WriteLine("6. Beginning Initial Gradient Magnitude Thresholding");
            double[,] gradientMagnitudeThreshold = ApplyGradientMagnitudeThreshold(thetaArray, gradientCombined);
            Bitmap gradientMagnitudeThresholdImage = DoubleArrayToBitmap(gradientMagnitudeThreshold);
            gradientMagnitudeThresholdImage.Save("./out/h.jpg");
            gradientMagnitudeThresholdImage.Dispose();

            Console.WriteLine("7. Beginning Secondary Min Max Thresholding");
            (double, bool)[,] doubleThresholdArray = ApplyDoubleThreshold(0.1, 0.3, gradientMagnitudeThreshold);

            double[,] doubleThresholdImageArray = new double[input.Height, input.Width];
            for (int i = 0; i < input.Height; i++) for (int j = 0; j < input.Width; j++) doubleThresholdImageArray[i, j] = doubleThresholdArray[i, j].Item1;
            Bitmap doubleThresholdImage = DoubleArrayToBitmap(doubleThresholdImageArray);
            doubleThresholdImage.Save("./out/i.jpg");
            doubleThresholdImage.Dispose();

            Console.WriteLine("8. Applying Hystersis");
            double[,] edgeTrackingHystersis = ApplyEdgeTrackingHystersis(doubleThresholdArray);
            Bitmap finalImage = DoubleArrayToBitmap(edgeTrackingHystersis);
            finalImage.Save("./out/j.jpg");
            finalImage.Dispose();

            Console.WriteLine("9. Embossing out image");
            double[,] embosArray = EmbosImage(edgeTrackingHystersis);
            Bitmap embosImage = DoubleArrayToBitmap(embosArray);
            embosImage.Save("./out/k.jpg");
            embosImage.Dispose();

            Console.WriteLine("10. Filling in the blanks");
            double[,] filledArray = FillImage(embosArray);
            Bitmap filledImage = DoubleArrayToBitmap(filledArray);
            filledImage.Save("./out/l.jpg");
            filledImage.Dispose();

            thing.Stop();
            Console.WriteLine($"Done, took {thing.ElapsedMilliseconds}ms");
            Console.ReadLine();
        }

        public static double[,] FillImage(double[,] imageArray)
        {
            double[,] result = imageArray;

            for (int i = 0; i < imageArray.GetLength(0); i++)
            {
                for (int j = 0; j < imageArray.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, 3, imageArray);
                    int count = 0;
                    foreach (double value in imageKernel.matrix)
                    {
                        if (value >= 255) count++;
                    }

                    if (count > 4) result[i, j] = 255;
                }
            }

            return result;
        }

        public static double[,] EmbosImage(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix embosMatrix = new Matrix(new double[,] {
                { -2, -1, 0 },
                { -1,  1, 1 },
                {  0,  1, 2 },
            });

            for (int i = 0; i < imageArray.GetLength(0); i++)
            {
                for (int j = 0; j < imageArray.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, 3, imageArray);
                    result[i, j] = Math.Abs(Matrix.Convolution(imageKernel, embosMatrix));
                }
            }

            return result;
        }

        public static Bitmap ConvertThetaToBitmap(double[,] angles)
        {
            Bitmap image = new Bitmap(angles.GetLength(1), angles.GetLength(0));

            for (int i = 0; i < angles.GetLength(0); i++)
            {
                for (int j = 0; j < angles.GetLength(1); j++)
                {
                    int x = (int)(
                        ((128 / (2 * Math.PI)) * angles[i, j]) + 128
                    );

                    image.SetPixel(j, i, Color.FromArgb(x, x, x));
                }
            }

            return image;

        }

        public static double[,] ApplyEdgeTrackingHystersis((double, bool)[,] arrayOfValues)
        {
            double[,] result = new double[arrayOfValues.GetLength(0), arrayOfValues.GetLength(1)];

            for (int i = 0; i < arrayOfValues.GetLength(0); i++)
            {
                for (int j = 0; j < arrayOfValues.GetLength(1); j++)
                {
                    if (arrayOfValues[i, j].Item2 == false)
                    {
                        (double, bool)[,] imageKernel = BuildKernel(j, i, 3, arrayOfValues);
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (imageKernel[k, l].Item2 == false) result[i, j] = 0;
                                else result[i, j] = 255;
                            }
                        }
                    }
                    else result[i, j] = 255;
                }
            }

            return result;
        }

        public static double[,] ApplyGradientMagnitudeThreshold(double[,] angles, double[,] magnitudes)
        {
            double[,] result = magnitudes;
            double[,] anglesInDegrees = ConvertThetaToDegrees(angles);

            for (int i = 0; i < anglesInDegrees.GetLength(0); i++)
            {
                for (int j = 0; j < anglesInDegrees.GetLength(1); j++)
                {
                    double[,] magnitudeKernel = BuildKernel(j, i, 3, magnitudes).matrix;

                    if (anglesInDegrees[i, j] < 22.5 || anglesInDegrees[i, j] >= 157.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[1, 2] || magnitudes[i, j] < magnitudeKernel[1, 0])
                        {
                            result[i, j] = 0;
                        }
                    }
                    else if (anglesInDegrees[i, j] >= 22.5 && anglesInDegrees[i, j] < 67.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 2] || magnitudes[i, j] < magnitudeKernel[2, 0])
                        {
                            result[i, j] = 0;
                        }
                    }
                    else if (anglesInDegrees[i, j] >= 67.5 && anglesInDegrees[i, j] < 112.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 1] || magnitudes[i, j] < magnitudeKernel[2, 1])
                        {
                            result[i, j] = 0;
                        }
                    }
                    else if (anglesInDegrees[i, j] >= 112.5 && anglesInDegrees[i, j] < 157.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 0] || magnitudes[i, j] < magnitudeKernel[2, 2])
                        {
                            result[i, j] = 0;
                        }
                    }
                    else throw new Exception();
                }
            }

            return result;
        }


        public static (double, bool)[,] ApplyDoubleThreshold(double l, double h, double[,] gradients)
        {
            double min = l * 255;
            double max = h * 255;

            (double, bool)[,] result = new (double, bool)[gradients.GetLength(0), gradients.GetLength(1)];

            for (int i = 0; i < gradients.GetLength(0); i++)
            {
                for (int j = 0; j < gradients.GetLength(1); j++)
                {
                    if (gradients[i, j] < min) result[i, j] = (0, false);
                    else if (gradients[i, j] > min && gradients[i, j] < max) result[i, j] = (gradients[i, j], false);
                    else if (gradients[i, j] > max) result[i, j] = (gradients[i, j], true);
                    else throw new Exception();
                }
            }

            return result;
        }

        public static double[,] ConvertThetaToDegrees(double[,] thetaArray)
        {
            double[,] result = new double[thetaArray.GetLength(0), thetaArray.GetLength(1)];
            for (int i = 0; i < thetaArray.GetLength(0); i++) for (int j = 0; j < thetaArray.GetLength(1); j++) result[i, j] = 180 * Math.Abs(thetaArray[i, j]) / Math.PI;
            return result;
        }

        public static double[,] CalculateTheta(double[,] gradX, double[,] gradY)
        {
            double[,] result = new double[gradX.GetLength(0), gradX.GetLength(1)];
            for (int i = 0; i < gradX.GetLength(0); i++) for (int j = 0; j < gradX.GetLength(1); j++) result[i, j] = Math.Atan2(gradY[i, j], gradX[i, j]);
            return result;
        }

        public static double[,] CalculateGradientCombined(double[,] gradX, double[,] gradY)
        {
            double[,] result = new double[gradX.GetLength(0), gradX.GetLength(1)];
            for (int i = 0; i < gradX.GetLength(0); i++) for (int j = 0; j < gradX.GetLength(1); j++) result[i, j] = Math.Sqrt(Math.Pow(gradX[i, j], 2) + Math.Pow(gradY[i, j], 2));
            return result;
        }

        public static double[,] CalculateGradientX(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix sobelX = new Matrix(new double[,] {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1 },
            });


            for (int i = 0; i < imageArray.GetLength(0); i++)
            {
                for (int j = 0; j < imageArray.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, 3, imageArray);
                    result[i, j] = Matrix.Convolution(imageKernel, sobelX);
                }
            }

            return result;
        }

        public static double[,] CalculateGradientY(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix sobelY = new Matrix(new double[,] {
                {  1,  2,  1 },
                {  0,  0,  0 },
                { -1, -2, -1 },
            });
            for (int i = 0; i < imageArray.GetLength(0); i++)
            {
                for (int j = 0; j < imageArray.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, 3, imageArray);
                    result[i, j] = Matrix.Convolution(imageKernel, sobelY);
                }
            }


            return result;
        }

        public static double[,] GaussianFilter(double sigma, int kernelSize, double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix gaussianKernel = GetGaussianKernel(kernelSize, sigma);

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, kernelSize, imageArray);
                    double sum = Matrix.Convolution(imageKernel, gaussianKernel);
                    result[i, j] = sum;
                }
            }

            return result;
        }

        public static Matrix GetGaussianKernel(int k, double sigma)
        {
            double[,] result = new double[k, k];
            int halfK = k / 2;

            double sum = 0;

            int cntY = -halfK;
            for (int i = 0; i < k; i++)
            {
                int cntX = -halfK;
                for (int j = 0; j < k; j++)
                {
                    result[halfK + cntY, halfK + cntX] = GetGaussianDistribution(cntX, cntY, sigma);
                    sum += result[halfK + cntY, halfK + cntX];
                    cntX++;
                }
                cntY++;
            }

            for (int i = 0; i < k; i++) for (int j = 0; j < k; j++) result[i, j] /= sum;
            return new Matrix(result);
        }


        public static Matrix BuildKernel(int x, int y, int k, double[,] grid)
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

            return new Matrix(kernel);
        }

        public static (double, bool)[,] BuildKernel(int x, int y, int k, (double, bool)[,] grid)
        {
            (double, bool)[,] kernel = new (double, bool)[k, k];

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

        public static double[,] BWFilter(Bitmap image)
        {
            double[,] result = new double[image.Height, image.Width];

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color c = image.GetPixel(j, i);
                    double value = c.R * 0.299 + c.G * 0.587 + c.B * 0.114;

                    result[i, j] = Bound(0, 255, value);
                }
            }

            return result;
        }

        public static int Bound(int l, int h, double v) => v > h ? h : (v < l ? l : (int)v);

        public static double GetGaussianDistribution(int x, int y, double sigma) =>
            1 / (2 * Math.PI * sigma * sigma) * Math.Exp(-((Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * sigma * sigma)));


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
    }

    public class Matrix
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public double[,] matrix { get; private set; }

        public Matrix(double[,] inputMatrix)
        {
            x = inputMatrix.GetLength(1);
            y = inputMatrix.GetLength(0);
            matrix = inputMatrix;
        }

        public static double Convolution(Matrix a, Matrix b)
        {
            if (a.x != a.y || b.x != a.x) throw new Exception();

            double[,] flippedB = new double[b.y, b.x];
            int l = b.x;
            for (int i = l - 1; i >= 0; i--)
            {
                for (int j = l - 1; j >= 0; j--)
                {
                    flippedB[b.y - (i + 1), b.x - (j + 1)] = b.matrix[i, j];
                }
            }


            double sum = 0;
            for (int i = 0; i < a.y; i++)
            {
                for (int j = 0; j < a.x; j++)
                {
                    sum += a.matrix[i, j] * flippedB[i, j];
                }
            }

            return sum;
        }
    }
}
