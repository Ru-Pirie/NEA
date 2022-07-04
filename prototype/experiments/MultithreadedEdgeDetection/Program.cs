using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace MultithreadedEdgeDetection
{
    public class Program {
        public static void Main(string[] args)
        {
            Bitmap image = new Bitmap("./image.jpg");
            if (image.Width < 400 || image.Width < 400) throw new Exception("Too small must be at least 400 x 400");
            if (image.Width % 2 == 1 || image.Height % 2 == 1)
                throw new Exception("Must be an even size im too lazy to make it work otherwise");

            Bitmap[] images = SplitImage(image);

            Task<double[,]>[] tasks = new Task<double[,]>[4];

            for (int i = 0; i < tasks.Length; i++)
            {
                // capture condition some weird fuckery going on in here
                int copyI = i;
                Console.WriteLine(copyI);
                images[i].Save($"image{i}.jpg");
                CannyDetection item = new CannyDetection(images[copyI], copyI);
                item.DoDetect();
            }

        }

        public static Bitmap[] SplitImage(Bitmap image)
        {
            Bitmap one = new Bitmap(image.Width / 2, image.Height / 2);
            Bitmap two = new Bitmap(image.Width / 2, image.Height / 2);
            Bitmap three = new Bitmap(image.Width / 2, image.Height / 2);
            Bitmap four = new Bitmap(image.Width / 2, image.Height / 2);

            for (int i = 0; i < image.Width / 2; i++)
            {
                for (int j = 0; j < image.Height / 2; j++)
                {
                    one.SetPixel(i, j, image.GetPixel(i, j));
                }
            }

            for (int i = image.Width / 2; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height / 2; j++)
                {
                    two.SetPixel(i - (image.Width / 2), j, image.GetPixel(i, j));
                }
            }

            for (int i = 0; i < image.Width / 2; i++)
            {
                for (int j = image.Height / 2; j < image.Height; j++)
                {
                    three.SetPixel(i, j - (image.Height / 2), image.GetPixel(i, j));
                }
            }

            for (int i = image.Width / 2; i < image.Width; i++)
            {
                for (int j = image.Height / 2; j < image.Height; j++)
                {
                    four.SetPixel(i - (image.Width / 2), j - (image.Height / 2), image.GetPixel(i , j));
                }
            }

            return new[] { one, two, three, four };
            
        }
    }


    
    public class CannyDetection
    {
        private Bitmap masterImage;
        private int id;

        public CannyDetection(Bitmap masterImage, int id)
        {
            this.masterImage = masterImage;
            this.id = id;
        }

        public double[,] DoDetect()
        {
            Console.WriteLine("Beginning Edge Detection...");
            Bitmap input = new Bitmap(masterImage);
            input.Save($"./out/a{id}.jpg");

            Console.WriteLine($"1. Converting to Black and White ({id})");
            double[,] bwArray = BWFilter(input);
            Bitmap bwImage = DoubleArrayToBitmap(bwArray);
            bwImage.Save($"./out/b{id}.jpg");
            bwImage.Dispose();

            Console.WriteLine($"2. Beginning Gaussian Filter ({id})");
            double[,] gaussianArray = GaussianFilter(1.4, 7, bwArray);
            Bitmap gaussianImage = DoubleArrayToBitmap(gaussianArray);
            gaussianImage.Save($"./out/c{id}.jpg");
            gaussianImage.Dispose();

            Console.WriteLine($"3. Beginning Gradient Calculations ({id})");

            Task<double[,]>[] tasks = new Task<double[,]>[2];
            tasks[0] = new Task<double[,]>(() => CalculateGradientX(gaussianArray));
            tasks[1] = new Task<double[,]>(() => CalculateGradientY(gaussianArray));

            foreach (var task in tasks) task.Start();
            Task.WaitAll(tasks);

            Bitmap gradientXImage = DoubleArrayToBitmap(tasks[0].Result);
            Bitmap gradientYImage = DoubleArrayToBitmap(tasks[1].Result);
            gradientXImage.Save($"./out/d{id}.jpg");
            gradientYImage.Save($"./out/e{id}.jpg");
            gradientXImage.Dispose();
            gradientYImage.Dispose();

            Console.WriteLine($"4. Beginning Total Gradient Calculations ({id})");
            double[,] gradientCombined = CalculateGradientCombined(tasks[0].Result, tasks[1].Result);
            Bitmap gradientCombinedImage = DoubleArrayToBitmap(gradientCombined);
            gradientCombinedImage.Save($"./out/f{id}.jpg");
            gradientCombinedImage.Dispose();

            Console.WriteLine($"5. Calculating Gradient Angles Calculations ({id})");
            double[,] thetaArray = CalculateTheta(tasks[0].Result, tasks[1].Result);
            Bitmap thetaImage = ConvertThetaToBitmap(thetaArray);
            thetaImage.Save($"./out/g{id}.jpg");
            thetaImage.Dispose();

            Console.WriteLine($"6. Beginning Initial Gradient Magnitude Thresholding ({id})");
            double[,] gradientMagnitudeThreshold = ApplyGradientMagnitudeThreshold(thetaArray, gradientCombined);
            Bitmap gradientMagnitudeThresholdImage = DoubleArrayToBitmap(gradientMagnitudeThreshold);
            gradientMagnitudeThresholdImage.Save($"./out/h{id}.jpg");
            gradientMagnitudeThresholdImage.Dispose();

            Console.WriteLine($"7. Beginning Secondary Min Max Thresholding ({id})");
            (double, bool)[,] doubleThresholdArray = ApplyDoubleThreshold(0.1, 0.3, gradientMagnitudeThreshold);

            double[,] doubleThresholdImageArray = new double[input.Height, input.Width];
            for (int i = 0; i < input.Height; i++) for (int j = 0; j < input.Width; j++) doubleThresholdImageArray[i, j] = doubleThresholdArray[i, j].Item1;
            Bitmap doubleThresholdImage = DoubleArrayToBitmap(doubleThresholdImageArray);
            doubleThresholdImage.Save($"./out/i{id}.jpg");
            doubleThresholdImage.Dispose();

            Console.WriteLine($"8. Applying Hystersis ({id})");
            double[,] edgeTrackingHystersis = ApplyEdgeTrackingHystersis(doubleThresholdArray);
            Bitmap finalImage = DoubleArrayToBitmap(edgeTrackingHystersis);
            finalImage.Save($"./out/j{id}.jpg");
            finalImage.Dispose();

            Console.WriteLine($"9. Fill out image ({id})");
            double[,] filledImage = EmbosImage(EmbosImage(edgeTrackingHystersis));
            Bitmap filledImageBitmap = DoubleArrayToBitmap(filledImage);
            filledImageBitmap.Save($"./out/k{id}.jpg");
            filledImageBitmap.Dispose();

            Console.WriteLine($"Done {id}");
            
            return edgeTrackingHystersis;
        }

        public double[,] EmbosImage(double[,] imageArray)
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

        public Bitmap ConvertThetaToBitmap(double[,] angles)
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

        public double[,] ApplyEdgeTrackingHystersis((double, bool)[,] arrayOfValues)
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

        public double[,] ApplyGradientMagnitudeThreshold(double[,] angles, double[,] magnitudes)
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


        public (double, bool)[,] ApplyDoubleThreshold(double l, double h, double[,] gradients)
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

        public double[,] ConvertThetaToDegrees(double[,] thetaArray)
        {
            double[,] result = new double[thetaArray.GetLength(0), thetaArray.GetLength(1)];
            for (int i = 0; i < thetaArray.GetLength(0); i++) for (int j = 0; j < thetaArray.GetLength(1); j++) result[i, j] = 180 * Math.Abs(thetaArray[i, j]) / Math.PI;
            return result;
        }

        public double[,] CalculateTheta(double[,] gradX, double[,] gradY)
        {
            double[,] result = new double[gradX.GetLength(0), gradX.GetLength(1)];
            for (int i = 0; i < gradX.GetLength(0); i++) for (int j = 0; j < gradX.GetLength(1); j++) result[i, j] = Math.Atan2(gradY[i, j], gradX[i, j]);
            return result;
        }

        public  double[,] CalculateGradientCombined(double[,] gradX, double[,] gradY)
        {
            double[,] result = new double[gradX.GetLength(0), gradX.GetLength(1)];
            for (int i = 0; i < gradX.GetLength(0); i++) for (int j = 0; j < gradX.GetLength(1); j++) result[i, j] = Math.Sqrt(Math.Pow(gradX[i, j], 2) + Math.Pow(gradY[i, j], 2));
            return result;
        }

        public  double[,] CalculateGradientX(double[,] imageArray)
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

        public  double[,] CalculateGradientY(double[,] imageArray)
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

        public  double[,] GaussianFilter(double sigma, int kernelSize, double[,] imageArray)
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

        public  Matrix GetGaussianKernel(int k, double sigma)
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


        public  Matrix BuildKernel(int x, int y, int k, double[,] grid)
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

        public  (double, bool)[,] BuildKernel(int x, int y, int k, (double, bool)[,] grid)
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

        public  double[,] BWFilter(Bitmap image)
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

        public  int Bound(int l, int h, double v) => v > h ? h : (v < l ? l : (int)v);

        public  double GetGaussianDistribution(int x, int y, double sigma) =>
            1 / (2 * Math.PI * sigma * sigma) * Math.Exp(-((Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * sigma * sigma)));


        public  Bitmap DoubleArrayToBitmap(double[,] input)
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
