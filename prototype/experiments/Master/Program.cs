using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Master
{
    internal class Program
    {
        public static Random gen = new Random();

        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            EdgeDetect();
            GetPaths();
            Combine();
            sw.Stop();
            Console.Clear();
            Console.WriteLine($"From start to finish this program took {sw.Elapsed}");
            Console.ReadLine();
        }

        public static void Combine()
        {
            Bitmap original = new Bitmap("./image.jpg");
            Bitmap mask = new Bitmap("./out/cleaned.jpg");
            if (original.Width != mask.Width || mask.Height != original.Height)
                throw new Exception("Images are not the same size");

            Bitmap output = new Bitmap(original);

            for (int i = 0; i < mask.Width; i++)
            {
                for (int j = 0; j < mask.Height; j++)
                {
                    Color pixel = mask.GetPixel(i, j);
                    if (pixel.R >= 10 && pixel.G >= 10 && pixel.B >= 10)
                        output.SetPixel(i, j, Color.FromArgb(0, 0, 255));
                }
            }

            output.Save("./output.jpg");
        }

        public static void GetPaths()

        {
            Bitmap input = new Bitmap("./out/final.jpg");
            Color[,] image = new Color[input.Height, input.Width];

            for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++)
                image[i, j] = input.GetPixel(j, i);

            List<Color> toReplaceColors = new List<Color>();
            List<Color> usedColors = new List<Color>();

            for (int i = 0; i < input.Height; i++)
            {
                for (int j = 0; j < input.Width; j++)
                {
                    int minX = input.Width, maxX = 0, minY = input.Height, maxY = 0;
                    double filled = 0;

                    Color randCol = Color.FromArgb(gen.Next(56, 256), gen.Next(56, 256), gen.Next(56, 256));
                    while (usedColors.Contains(randCol))
                        randCol = Color.FromArgb(gen.Next(56, 256), gen.Next(56, 256), gen.Next(56, 256));

                    Queue<(int, int)> queue = new Queue<(int, int)>();
                    queue.Enqueue((i, j));

                    while (queue.Count > 0)
                    {
                        (int, int) coord = queue.Dequeue();
                        if (image[coord.Item1, coord.Item2] == Color.FromArgb(0, 0, 0))
                        {
                            image[coord.Item1, coord.Item2] = randCol;
                            input.SetPixel(coord.Item2, coord.Item1, image[coord.Item1, coord.Item2]);

                            if (coord.Item1 > 0) queue.Enqueue((coord.Item1 - 1, coord.Item2));
                            if (coord.Item2 > 0) queue.Enqueue((coord.Item1, coord.Item2 - 1));
                            if (coord.Item1 < input.Height - 1) queue.Enqueue((coord.Item1 + 1, coord.Item2));
                            if (coord.Item2 < input.Width - 1) queue.Enqueue((coord.Item1, coord.Item2 + 1));

                            if (!usedColors.Contains(randCol)) usedColors.Add(randCol);

                            filled++;
                        }
                        else if (image[coord.Item1, coord.Item2] == Color.FromArgb(255, 255, 255))
                        {
                            image[coord.Item1, coord.Item2] = Color.FromArgb(1, 1, 1);
                            input.SetPixel(coord.Item2, coord.Item1, image[coord.Item1, coord.Item2]);
                        }

                        if (coord.Item1 > maxY) maxY = coord.Item1;
                        if (coord.Item2 > maxX) maxX = coord.Item2;
                        if (coord.Item1 < minY) minY = coord.Item1;
                        if (coord.Item2 < minX) minX = coord.Item2;
                    }

                    double totalSquares = (maxX - minX) * (maxY - minY);
                    if (filled / totalSquares > 0.2) toReplaceColors.Add(randCol);
                }
            }

            input.Save("./out/filled.jpg");

            for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++)
                if (toReplaceColors.Contains(image[i, j]))
                    input.SetPixel(j, i, Color.FromArgb(1, 1, 1));

            input.Save("./out/cleaned.jpg");
        }


        public static void EdgeDetect()
        {
            System.IO.Directory.CreateDirectory("./out");
            var thing = System.Diagnostics.Stopwatch.StartNew();
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
                CannyDetection item = new CannyDetection();
                Task<double[,]> task = new Task<double[,]>(() => item.DoDetect(images[copyI], copyI + 1));
                task.Start();
                tasks[i] = task;
            }

            Task.WaitAll(tasks);
            thing.Stop();

            double[,] partA = new double[image.Height / 2, image.Width];
            double[,] partB = new double[image.Height / 2, image.Width];
            for (int i = 0; i < tasks[0].Result.GetLength(0); i++)
            {
                for (int j = 0; j < tasks[0].Result.GetLength(1); j++)
                    partA[i, j] = tasks[0].Result[i, j];

                for (int y = 0; y < tasks[1].Result.GetLength(1); y++)
                    partA[i, y + tasks[0].Result.GetLength(1)] = tasks[1].Result[i, y];
            }

            for (int i = 0; i < tasks[2].Result.GetLength(0); i++)
            {
                for (int j = 0; j < tasks[2].Result.GetLength(1); j++)
                    partB[i, j] = tasks[2].Result[i, j];

                for (int y = 0; y < tasks[3].Result.GetLength(1); y++)
                    partB[i, y + tasks[2].Result.GetLength(1)] = tasks[3].Result[i, y];
            }

            double[,] final = new double[image.Height, image.Width];
            for (int i = 0; i < image.Height; i++)
            {
                if (i < image.Height / 2)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        final[i, j] = partA[i, j];
                    }
                }
                else
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        final[i, j] = partB[i - image.Height / 2, j];
                    }
                }
            }

            Bitmap finalImage = CannyDetection.DoubleArrayToBitmap(final);
            finalImage.Save("./out/final.jpg");

            Console.WriteLine($"Done, took {thing.ElapsedMilliseconds}ms");
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
                    four.SetPixel(i - (image.Width / 2), j - (image.Height / 2), image.GetPixel(i, j));
                }
            }

            return new[] { one, two, three, four };

        }
    }

    public class CannyDetection
    {
        private Bitmap _image;
        
        private int _kernelSize = 5;
        private double _redRatio = 0.299, _greenRatio = 0.587, _blueRatio = 0.114, _sigma = 1.4, _lowerThreshold = 0.1, _upperThreshold = 0.3;

        public double[,] DoDetect(Bitmap masterImage, int id)
        {
            Console.WriteLine("Beginning Edge Detection...");
            Bitmap input = new Bitmap(masterImage);
            _image = input;
            input.Save($"./out/a{id}.jpg");

            Console.WriteLine($"1. Converting to Black and White ({id})");
            double[,] bwArray = BWFilter(input);
            Bitmap bwImage = DoubleArrayToBitmap(bwArray);
            bwImage.Save($"./out/b{id}.jpg");
            bwImage.Dispose();

            Console.WriteLine($"2. Beginning Gaussian Filter ({id})");
            double[,] gaussianArray = GaussianFilter(bwArray);
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
            (double, bool)[,] doubleThresholdArray = ApplyDoubleThreshold(gradientMagnitudeThreshold);

            double[,] doubleThresholdImageArray = new double[input.Height, input.Width];
            for (int i = 0; i < input.Height; i++)
            for (int j = 0; j < input.Width; j++)
                doubleThresholdImageArray[i, j] = doubleThresholdArray[i, j].Item1;
            Bitmap doubleThresholdImage = DoubleArrayToBitmap(doubleThresholdImageArray);
            doubleThresholdImage.Save($"./out/i{id}.jpg");
            doubleThresholdImage.Dispose();

            Console.WriteLine($"8. Applying Hystersis ({id})");
            double[,] edgeTrackingHystersis = ApplyEdgeTrackingHystersis(doubleThresholdArray);
            Bitmap finalImage = DoubleArrayToBitmap(edgeTrackingHystersis);
            finalImage.Save($"./out/j{id}.jpg");
            finalImage.Dispose();

            Console.WriteLine("9. Embossing out image");
            double[,] embosArray = EmbosImage(edgeTrackingHystersis);
            Bitmap embosImage = DoubleArrayToBitmap(embosArray);
            embosImage.Save($"./out/k{id}.jpg");
            embosImage.Dispose();

            Console.WriteLine("10. Filling in the blanks");
            double[,] filledArray = FillImage(embosArray);
            Bitmap filledImage = DoubleArrayToBitmap(filledArray);
            filledImage.Save($"./out/l{id}.jpg");
            filledImage.Dispose();

            Console.WriteLine($"Done {id}");

            return filledArray;
        }

        public double[,] FillImage(double[,] imageArray)
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

        public double[,] EmbosImage(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix embosMatrix = new Matrix(new double[,]
            {
                { -2, -1, 0 },
                { -1, 1, 1 },
                { 0, 1, 2 },
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

        public (double, bool)[,] ApplyDoubleThreshold(double[,] gradients)
        {
            double min = _lowerThreshold * 255;
            double max = _upperThreshold * 255;

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
                            result[i, j] = 0;
                    }
                    else if (anglesInDegrees[i, j] >= 22.5 && anglesInDegrees[i, j] < 67.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 2] || magnitudes[i, j] < magnitudeKernel[2, 0])
                            result[i, j] = 0;
                    }
                    else if (anglesInDegrees[i, j] >= 67.5 && anglesInDegrees[i, j] < 112.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 1] || magnitudes[i, j] < magnitudeKernel[2, 1])
                            result[i, j] = 0;
                    }
                    else if (anglesInDegrees[i, j] >= 112.5 && anglesInDegrees[i, j] < 157.5)
                    {
                        if (magnitudes[i, j] < magnitudeKernel[0, 0] || magnitudes[i, j] < magnitudeKernel[2, 2])
                            result[i, j] = 0;
                    }
                    else throw new Exception();
                }
            }

            return result;
        }

        public double[,] ConvertThetaToDegrees(double[,] thetaArray)
        {
            double[,] result = new double[thetaArray.GetLength(0), thetaArray.GetLength(1)];
            for (int i = 0; i < thetaArray.GetLength(0); i++)
            for (int j = 0; j < thetaArray.GetLength(1); j++)
                result[i, j] = 180 * Math.Abs(thetaArray[i, j]) / Math.PI;
            return result;
        }

        // Implement Atan2 myself?
        public double[,] CalculateTheta(double[,] gradX, double[,] gradY)
        {
            double[,] result = new double[gradX.GetLength(0), gradX.GetLength(1)];
            for (int i = 0; i < gradX.GetLength(0); i++)
            for (int j = 0; j < gradX.GetLength(1); j++)
                result[i, j] = Math.Atan2(gradY[i, j], gradX[i, j]);
            return result;
        }

        public double[,] CalculateGradientCombined(double[,] gradX, double[,] gradY)
        {
            double[,] result = new double[gradX.GetLength(0), gradX.GetLength(1)];
            for (int i = 0; i < gradX.GetLength(0); i++)
            for (int j = 0; j < gradX.GetLength(1); j++)
                result[i, j] = Math.Sqrt(Math.Pow(gradX[i, j], 2) + Math.Pow(gradY[i, j], 2));
            return result;
        }

        public double[,] CalculateGradientX(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix sobelX = new Matrix(new double[,]
            {
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

        public double[,] CalculateGradientY(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix sobelY = new Matrix(new double[,]
            {
                { 1, 2, 1 },
                { 0, 0, 0 },
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

        private double[,] GaussianFilter(double[,] imageArray)
        {
            double[,] result = new double[imageArray.GetLength(0), imageArray.GetLength(1)];

            Matrix gaussianKernel = GetGaussianKernel();

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Matrix imageKernel = BuildKernel(j, i, _kernelSize, imageArray);
                    double sum = Matrix.Convolution(imageKernel, gaussianKernel);
                    result[i, j] = sum;
                }
            }

            return result;
        }

        public Matrix GetGaussianKernel()
        {
            double[,] result = new double[_kernelSize, _kernelSize];
            int halfK = _kernelSize / 2;

            double sum = 0;

            int cntY = -halfK;
            for (int i = 0; i < _kernelSize; i++)
            {
                int cntX = -halfK;
                for (int j = 0; j < _kernelSize; j++)
                {
                    result[halfK + cntY, halfK + cntX] = GetGaussianDistribution(cntX, cntY, _sigma);
                    sum += result[halfK + cntY, halfK + cntX];
                    cntX++;
                }

                cntY++;
            }

            for (int i = 0; i < _kernelSize; i++)
            for (int j = 0; j < _kernelSize; j++)
                result[i, j] /= sum;
            return new Matrix(result);
        }

        private double[,] BWFilter(Bitmap image)
        {
            double[,] result = new double[image.Height, image.Width];

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color c = image.GetPixel(j, i);
                    double value = c.R * _redRatio + c.G * _greenRatio + c.B * _blueRatio;

                    result[i, j] = Bound(0, 255, value);
                }
            }

            return result;
        }

        public double[,] CombineQuadrants(double[,] a, double[,] b, double[,] c, double[,] d)
        {
            double[,] partA = new double[_image.Height / 2, _image.Width];
            double[,] partB = new double[_image.Height / 2, _image.Width];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                    partA[i, j] = a[i, j];

                for (int y = 0; y < b.GetLength(1); y++)
                    partA[i, y + a.GetLength(1)] = b[i, y];
            }

            for (int i = 0; i < c.GetLength(0); i++)
            {
                for (int j = 0; j < c.GetLength(1); j++)
                    partB[i, j] = c[i, j];

                for (int y = 0; y < d.GetLength(1); y++)
                    partB[i, y + c.GetLength(1)] = d[i, y];
            }

            double[,] final = new double[_image.Height, _image.Width];
            for (int i = 0; i < _image.Height; i++)
            {
                if (i < _image.Height / 2)
                {
                    for (int j = 0; j < _image.Width; j++)
                    {
                        final[i, j] = partA[i, j];
                    }
                }
                else
                {
                    for (int j = 0; j < _image.Width; j++)
                    {
                        final[i, j] = partB[i - _image.Height / 2, j];
                    }
                }
            }

            return final;
        }

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

        public double GetGaussianDistribution(int x, int y, double sigma) =>
            1 / (2 * Math.PI * sigma * sigma) * Math.Exp(-((Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * sigma * sigma)));

        public static int Bound(int l, int h, double v) => v > h ? h : v < l ? l : (int)v;

        public Matrix BuildKernel(int x, int y, int k, double[,] grid)
        {
            double[,] kernel = new double[k, k];

            int halfK = k / 2;

            for (int i = 0; i < k; i++)
            for (int j = 0; j < k; j++)
                kernel[i, j] = grid[y, x];

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

        private (double, bool)[,] BuildKernel(int x, int y, int k, (double, bool)[,] grid)
        {
            (double, bool)[,] kernel = new (double, bool)[k, k];

            int halfK = k / 2;

            for (int i = 0; i < k; i++)
            for (int j = 0; j < k; j++)
                kernel[i, j] = grid[y, x];

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