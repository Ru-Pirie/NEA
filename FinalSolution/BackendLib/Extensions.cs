using BackendLib.Datatypes;
using System;
using System.Drawing;

namespace BackendLib
{
    public static class Extensions
    {
        public static Bitmap ToBitmap(this double[,] array)
        {
            Bitmap output = new Bitmap(array.GetLength(1), array.GetLength(0));

            for (int y = 0; y < array.GetLength(0); y++)
            {
                for (int x = 0; x < array.GetLength(1); x++)
                {
                    int boundedPixel = (int)Utility.Bound(0, 255, array[y, x]);
                    output.SetPixel(x, y, Color.FromArgb(boundedPixel, boundedPixel, boundedPixel));
                }
            }

            return output;
        }

        public static double[,] ToDoubles(this Bitmap image, Func<Color, double> getPixelFunction)
        {
            double[,] result = new double[image.Height, image.Width];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    result[y, x] = getPixelFunction(image.GetPixel(x, y));
                }
            }

            return result;
        }

        public static Bitmap ToBitmap(this Structures.RGB[,] array)
        {
            Bitmap output = new Bitmap(array.GetLength(1), array.GetLength(0));

            for (int y = 0; y < array.GetLength(0); y++)
            {
                for (int x = 0; x < array.GetLength(1); x++)
                {
                    output.SetPixel(x, y, Color.FromArgb((int)array[y, x].R, (int)array[y, x].G, (int)array[y, x].B));
                }
            }

            return output;
        }

        public static Graph<Structures.Coord> ToGraph(this double[,] doubles)
        {
            Graph<Structures.Coord> output = new Graph<Structures.Coord>();
            Kernel<double> masterKernel = new Kernel<double>(doubles);

            for (int y = 0; y < doubles.GetLength(0); y++)
            {
                for (int x = 0; x < doubles.GetLength(1); x++)
                {
                    Structures.Coord tempCord = new Structures.Coord { X = x, Y = y };
                    output.AddNode(tempCord);

                    double[,] surroundingDoubles = masterKernel.Constant(x, y, 3, 0);

                    bool found = false;

                    if (doubles[y, x] == 255)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (surroundingDoubles[i / 3, i % 3] != 0 && i != 4)
                            {
                                output.AddConnection(tempCord, new Structures.Coord { X = (x + (i % 3)) - 1, Y = (y + (i / 3)) - 1 });
                                found = true;
                            }
                        }
                    }

                    if (!found) output.RemoveNode(tempCord);
                }
            }

            return output;
        }

        // To ensure compatibility with BITMAP
        public static void SetPixel(this Structures.RGB[,] pixels, int x, int y, Structures.RGB toSetPixel) => pixels[y, x] = toSetPixel;

        public static Structures.RGB GetPixel(this Structures.RGB[,] pixels, int x, int y) => pixels[y, x];
    }
}
