using BackendLib.Datatypes;
using System;
using System.Linq;

namespace BackendLib.Processing
{
    public class Post
    {
        private double[,] _imageDoubles;

        public Post(double[,] input)
        {
            _imageDoubles = input;
        }

        public void Start(int embossCount)
        {
            if (embossCount <= 0) _imageDoubles = FillPixelGaps(_imageDoubles);
            else
            {
                for (int i = 0; i < embossCount; i++)
                {
                    _imageDoubles = FillPixelGaps(EmbossImage(_imageDoubles));
                }
            }
        }

        private double[,] EmbossImage(double[,] input)
        {
            double[,] result = new double[input.GetLength(0), input.GetLength(1)];

            Matrix embossMatrix = new Matrix(new double[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } });
            Kernel<double> masterKernel = new Kernel<double>(input);

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    Matrix imageKernel = new Matrix(masterKernel.Duplication(x, y, 3));
                    result[y, x] = Math.Abs(Matrix.Convolution(imageKernel, embossMatrix));
                }
            }

            return result;
        }

        private double[,] FillPixelGaps(double[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];
            Kernel<double> masterKernel = new Kernel<double>(input);


            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    Matrix imageKernel = new Matrix(masterKernel.Duplication(x, y, 3));
                    int count = imageKernel.Cast<double>().Count(value => value >= 255);
                    if (count > 4) output[y, x] = 255;
                }
            }

            return output;
        }


        public double[,] Result() => _imageDoubles;

    }
}
