using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Processing
{
    public class RoadDetection
    {
        private Bitmap _filledBitmap;
        private Bitmap _pathBitmap;
        private readonly double[,] _imageDoubles;
        private readonly double _threshold;
        private Random _gen = new Random();

        public RoadDetection(double[,] imageDoubles, double threshold)
        {
            _imageDoubles = imageDoubles;
            _threshold = threshold;
        }

        public void Start(Action updateAction)
        {
            List<Color> toRemoveColors = FillImage(updateAction);
            RemoveColor(toRemoveColors, updateAction);
        }

        private List<Color> FillImage(Action updateAction)
        {
            Color[,] tempImage = new Color[_imageDoubles.GetLength(0), _imageDoubles.GetLength(1)];

            for (int y = 0; y < _imageDoubles.GetLength(0); y++)
            for (int x = 0; x < _imageDoubles.GetLength(1); x++)
                tempImage[y, x] = Color.FromArgb((int)_imageDoubles[y, x], (int)_imageDoubles[y, x], (int)_imageDoubles[y, x]);

            List<Color> toReplaceColors = new List<Color>();
            List<Color> usedColors = new List<Color>();

            _filledBitmap = _imageDoubles.ToBitmap();

            for (int y = 0; y < _imageDoubles.GetLength(0); y++)
            {
                for (int x = 0; x < _imageDoubles.GetLength(1); x++)
                {
                    if ((((y + 1) * (x + 1)) / 100) % 100 == 0) updateAction();

                    int minX = _imageDoubles.GetLength(1), maxX = 0, minY = _imageDoubles.GetLength(0), maxY = 0;
                    double filled = 0;

                    Color randCol = Color.FromArgb(_gen.Next(56, 256), _gen.Next(56, 256), _gen.Next(56, 256));
                    while (usedColors.Contains(randCol))
                        randCol = Color.FromArgb(_gen.Next(56, 256), _gen.Next(56, 256), _gen.Next(56, 256));

                    Datatypes.Queue<(int, int)> queue = new Datatypes.Queue<(int, int)>();
                    queue.Enqueue((y, x));

                    while (queue.Size > 0)
                    {
                        (int, int) cord = queue.Dequeue();
                        if (tempImage[cord.Item1, cord.Item2] == Color.FromArgb(0, 0, 0))
                        {
                            tempImage[cord.Item1, cord.Item2] = randCol;
                            _filledBitmap.SetPixel(cord.Item2, cord.Item1, tempImage[cord.Item1, cord.Item2]);

                            if (cord.Item1 > 0) queue.Enqueue((cord.Item1 - 1, cord.Item2));
                            if (cord.Item2 > 0) queue.Enqueue((cord.Item1, cord.Item2 - 1));
                            if (cord.Item1 < _filledBitmap.Height - 1) queue.Enqueue((cord.Item1 + 1, cord.Item2));
                            if (cord.Item2 < _filledBitmap.Width - 1) queue.Enqueue((cord.Item1, cord.Item2 + 1));

                            if (!usedColors.Contains(randCol)) usedColors.Add(randCol);

                            filled++;
                        }
                        else if (tempImage[cord.Item1, cord.Item2] == Color.FromArgb(255, 255, 255))
                        {
                            tempImage[cord.Item1, cord.Item2] = Color.FromArgb(1, 1, 1);
                            _filledBitmap.SetPixel(cord.Item2, cord.Item1, tempImage[cord.Item1, cord.Item2]);
                        }

                        if (cord.Item1 > maxY) maxY = cord.Item1;
                        if (cord.Item2 > maxX) maxX = cord.Item2;
                        if (cord.Item1 < minY) minY = cord.Item1;
                        if (cord.Item2 < minX) minX = cord.Item2;
                    }

                    double totalSquares = (maxX - minX) * (maxY - minY);
                    if (filled / totalSquares > _threshold) toReplaceColors.Add(randCol);
                }
            }

            return toReplaceColors;
        }

        private void RemoveColor(List<Color> toRemove, Action updateAction)
        {
            _pathBitmap = _filledBitmap;

            for (int y = 0; y < _pathBitmap.Height; y++)
            {
                for (int x = 0; x < _pathBitmap.Width; x++)
                {
                    if ((((y + 1) * (x + 1)) / 100) % 100 == 0) updateAction();
                    if (toRemove.Contains(_pathBitmap.GetPixel(x, y)))
                    {
                        _pathBitmap.SetPixel(x, y, Color.FromArgb(1, 1, 1));
                    }
                }
            }

            for (int i = 0; i < _pathBitmap.Height; i++)
            {
                for (int j = 0; j < _pathBitmap.Width; j++)
                {
                    if ((((i + 1) * (j + 1)) / 100) % 100 == 0) updateAction();
                    if (_pathBitmap.GetPixel(j, i) == Color.FromArgb(1, 1, 1))
                        _pathBitmap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                }
            }
        }

        public Structures.RoadResult Result() => new Structures.RoadResult
        { 
            FilledBitmap = _filledBitmap,
            PathBitmap = _pathBitmap
        };

    }
}
