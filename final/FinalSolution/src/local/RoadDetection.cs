using System;
using System.Collections.Generic;
using System.Drawing;

namespace FinalSolution.src.utility
{
    internal class RoadDetection
    {
        private Bitmap _image;
        private Bitmap _filledImage;

        private Random _gen = new Random();

        public RoadDetection(Bitmap image)
        {
            _image = image;
        }

        public void Start(double threshold)
        {
            Menu.SetupProgressBar("Filling Image", (_image.Height * _image.Width) / 100);

            List<Color> toReplaceColors = FillImage(threshold);
            RemoveColour(toReplaceColors);
        }

        public Bitmap[] Result() => new[] { _image, _filledImage };

        private List<Color> FillImage(double threshold)
        {
            Color[,] image = new Color[_image.Height, _image.Width];

            for (int i = 0; i < _image.Height; i++)
                for (int j = 0; j < _image.Width; j++)
                    image[i, j] = _image.GetPixel(j, i);

            List<Color> toReplaceColors = new List<Color>();
            List<Color> usedColors = new List<Color>();

            for (int i = 0; i < _image.Height; i++)
            {
                for (int j = 0; j < _image.Width; j++)
                {
                    if ((((i + 1) * (j + 1)) / 100) % 100 == 0) Menu.UpdateProgressBar();

                    int minX = _image.Width, maxX = 0, minY = _image.Height, maxY = 0;
                    double filled = 0;

                    Color randCol = Color.FromArgb(_gen.Next(56, 256), _gen.Next(56, 256), _gen.Next(56, 256));
                    while (usedColors.Contains(randCol))
                        randCol = Color.FromArgb(_gen.Next(56, 256), _gen.Next(56, 256), _gen.Next(56, 256));

                    datatypes.Queue<(int, int)> queue = new datatypes.Queue<(int, int)>();
                    queue.Enqueue((i, j));

                    while (queue.Size > 0)
                    {
                        (int, int) coord = queue.Dequeue();
                        if (image[coord.Item1, coord.Item2] == Color.FromArgb(0, 0, 0))
                        {
                            image[coord.Item1, coord.Item2] = randCol;
                            _image.SetPixel(coord.Item2, coord.Item1, image[coord.Item1, coord.Item2]);

                            if (coord.Item1 > 0) queue.Enqueue((coord.Item1 - 1, coord.Item2));
                            if (coord.Item2 > 0) queue.Enqueue((coord.Item1, coord.Item2 - 1));
                            if (coord.Item1 < _image.Height - 1) queue.Enqueue((coord.Item1 + 1, coord.Item2));
                            if (coord.Item2 < _image.Width - 1) queue.Enqueue((coord.Item1, coord.Item2 + 1));

                            if (!usedColors.Contains(randCol)) usedColors.Add(randCol);

                            filled++;
                        }
                        else if (image[coord.Item1, coord.Item2] == Color.FromArgb(255, 255, 255))
                        {
                            image[coord.Item1, coord.Item2] = Color.FromArgb(1, 1, 1);
                            _image.SetPixel(coord.Item2, coord.Item1, image[coord.Item1, coord.Item2]);
                        }

                        if (coord.Item1 > maxY) maxY = coord.Item1;
                        if (coord.Item2 > maxX) maxX = coord.Item2;
                        if (coord.Item1 < minY) minY = coord.Item1;
                        if (coord.Item2 < minX) minX = coord.Item2;
                    }

                    double totalSquares = (maxX - minX) * (maxY - minY);
                    if (filled / totalSquares > threshold) toReplaceColors.Add(randCol);
                }
            }

            _filledImage = _image;

            return toReplaceColors;
        }

        private void RemoveColour(List<Color> toRemove)
        {
            Menu.SetupProgressBar("Removing colours which have to much area", (_image.Height * _image.Width) / 100);

            for (int i = 0; i < _image.Height; i++)
            {
                for (int j = 0; j < _image.Width; j++)
                {
                    if ((((i + 1) * (j + 1)) / 100) % 100 == 0) Menu.UpdateProgressBar();
                    if (toRemove.Contains(_image.GetPixel(j, i)))
                    {
                        _image.SetPixel(j, i, Color.FromArgb(1, 1, 1));
                    }
                }
            }

            Menu.SetupProgressBar("Setting non black pixels back to black", (_image.Height * _image.Width) / 100);

            for (int i = 0; i < _image.Height; i++)
            {
                for (int j = 0; j < _image.Width; j++)
                {
                    if ((((i + 1) * (j + 1)) / 100) % 100 == 0) Menu.UpdateProgressBar();

                    if (_image.GetPixel(j, i) == Color.FromArgb(1, 1, 1))
                        _image.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                }
            }
        }






    }
}
