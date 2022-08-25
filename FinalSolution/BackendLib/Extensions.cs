using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
