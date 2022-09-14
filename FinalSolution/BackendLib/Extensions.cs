using BackendLib.Datatypes;
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

        public static Graph<Structures.Cord> ToGraph(this double[,] doubles)
        {
            Graph<Structures.Cord> output = new Graph<Structures.Cord>();
            Kernel<double> masterKernel = new Kernel<double>(doubles);

            for (int y = 0; y < doubles.GetLength(0); y++)
            {
                for (int x = 0; x < doubles.GetLength(1); x++)
                {
                    Structures.Cord tempCord = new Structures.Cord { X = x, Y = y };
                    output.AddNode(tempCord);

                    double[,] surroundingDoubles = masterKernel.Constant(x, y, 3, 0);
                    if (doubles[y, x] == 255)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (surroundingDoubles[i / 3, i % 3] != 0 && i != 4)
                                output.AddConnection(tempCord, new Structures.Cord { X = (x + (i % 3)) - 1, Y = (y + (i / 3)) - 1 });
                        }
                    }
                }
            }

            return output;
        }

    }
}
