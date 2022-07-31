using FinalSolution.src.utility;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using FinalSolution.src.local;
using FinalSolution.src.utility.datatypes;

namespace FinalSolution.src.local
{
    internal class ProcessImage
    {
        private Bitmap _input;
        private Bitmap _output;

        private readonly string _extentionPattern = @"^(\d|\w|(\\)|:){1,}.(jpg|bmp|exif|png|tiff)$";
        private string _path;

        public ProcessImage(string path)
        {
            _path = path;
            if (!IsImage()) throw new Exception("File supplied was not an image.");
        }

        public void Start()
        {
            _input = new Bitmap(_path);

            if (_input.Width % 2 != 0 || _input.Height % 2 != 0)
            {
                Log.Warn("Image is not of even size, correcting.");
                _output = new Bitmap(_input.Width / 2 * 2, _input.Height / 2 * 2);
                for (int i = 0; i < _output.Height; i++) for (int j = 0; j < _output.Width; j++) _output.SetPixel(j, i, _input.GetPixel(j, i));
            }
            else
            {
                _output = _input;
            }
        }

        public Bitmap Original() => _input;
        public Bitmap Result() => _output;
        private bool IsImage() => Regex.IsMatch(_path, _extentionPattern);

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

        public static double[,] FortifyImage(double[,] image, int itterations = 1)
        {
            for (int i = 0; i < itterations; i++)
            {
                Log.Event($"Embossing image (Iteration: {i+1}/{itterations})");
                image = EmbosImage(image);
                Log.Event($"Filling image (Iteration: {i + 1}/{itterations})");
                image = FillImage(image);
            }

            Log.End($"Fortification Complete");

            return image;
        }

        private static double[,] EmbosImage(double[,] image)
        {
            double[,] result = new double[image.GetLength(0), image.GetLength(1)];

            Matrix embosMatrix = new Matrix(new double[,]
            {
                { -2, -1, 0 },
                { -1, 1, 1 },
                { 0, 1, 2 },
            });

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    Matrix imageKernel = CannyEdgeDetection.BuildKernel(j, i, 3, image);
                    result[i, j] = Math.Abs(Matrix.Convolution(imageKernel, embosMatrix));
                }
            }

            return result;
        }
        private static double[,] FillImage(double[,] image)
        {
            double[,] result = image;

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    Matrix imageKernel = CannyEdgeDetection.BuildKernel(j, i, 3, image);
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

        public static void InvertImage(ref double[,] image)
        {
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    if (image[i, j] == 0) image[i, j] = 255;
                    else image[i, j] = 0;
                }
            }
        }
    }
}
