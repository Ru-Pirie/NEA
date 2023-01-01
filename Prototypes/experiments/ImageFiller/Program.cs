using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiller
{
    internal class Program
    {
        public static Random gen = new Random();

        static void Main(string[] args)
        {
            Bitmap input = new Bitmap("./image.jpg");
            Color[,] image = new Color[input.Height, input.Width];

            for (int i = 0; i < input.Height; i++) for (int j = 0; j < input.Width; j++) image[i, j] = input.GetPixel(j, i);
     
            List<Color> toReplaceColors = new List<Color>();
            List<Color> usedColors = new List<Color>();

            for (int i = 0; i < input.Height; i++)
            {
                for (int j = 0; j < input.Width; j++)
                {
                    int minX = input.Width, maxX = 0, minY = input.Height, maxY = 0;
                    double filled = 0;

                    Color randCol = Color.FromArgb(gen.Next(56, 256), gen.Next(56, 256), gen.Next(56, 256));
                    while (usedColors.Contains(randCol)) randCol = Color.FromArgb(gen.Next(56, 256), gen.Next(56, 256), gen.Next(56, 256));

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
                        } else if (image[coord.Item1, coord.Item2] == Color.FromArgb(255, 255, 255))
                        {
                            image[coord.Item1, coord.Item2] = Color.FromArgb(1,1,1);
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
            
            input.Save("filled.jpg");

            for (int i = 0; i < input.Height; i++) for (int j = 0; j < input.Width; j++) if (toReplaceColors.Contains(image[i, j])) input.SetPixel(j, i, Color.FromArgb(1, 1, 1));

            input.Save("cleaned.jpg");
        }

        public static Color[,] GetKernel(Color[,] grid, int x, int y, int k)
        {
            Color[,] kernel = new Color[k, k];

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
    }
}
