using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap original = new Bitmap("./original.jpg");
            Bitmap mask = new Bitmap("./mask.jpg");
            if (original.Width != mask.Width || mask.Height != original.Height) throw new Exception("Images are not the same size");

            Bitmap output = new Bitmap(original);

            for (int i = 0; i < mask.Width; i++)
            {
                for (int j = 0; j < mask.Height; j++)
                {
                    Color pixel = mask.GetPixel(i, j);
                    if (pixel.R >= 10 && pixel.G >= 10 && pixel.B >= 10) output.SetPixel(i, j, Color.FromArgb(0, 0, 255));
                }
            }

            output.Save("./output.jpg");
        }
    }
}
