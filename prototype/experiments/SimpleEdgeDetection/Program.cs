using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace SimpleEdgeDetection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap input = new Bitmap("images/in.png");

            Bitmap output = BWFilter(input);
            output.Save("images/bw.png");

            Math.Exp(1);
        }

        static Bitmap BWFilter(Bitmap image)
        {
            Bitmap output = new Bitmap(image.Width, image.Height);

            // loop through image pixel by pixel
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color inPixel = image.GetPixel(x, y);
                    int outPixelColor = (int)((0.299 * inPixel.R) + (0.587 * inPixel.G) + (0.114 * inPixel.B));
                    outPixelColor = outPixelColor > 255 ? 255 : (outPixelColor < 0 ? 0 : outPixelColor);
                    output.SetPixel(x, y, Color.FromArgb(outPixelColor, outPixelColor, outPixelColor));
                }
            }

            return output;
        }

        static Bitmap GuassianFilter(Bitmap image)
        {
            return new Bitmap(0, 0);
        }

    }
}
