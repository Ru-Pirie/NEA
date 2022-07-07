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
                    if (mask.GetPixel(i, j).R >= 128) output.SetPixel(i, j, Color.FromArgb(255, 0, 0));
                }
            }

            output.Save("./output.jpg");
        }
    }
}
