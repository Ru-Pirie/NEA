using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendLib.Datatypes;
using BackendLib.Exceptions;

namespace BackendLib
{
    internal class Kernel<T>
    {
        private readonly T[,] _image;
        int _width, _height;

        public Kernel(T[,] image)
        {
            _image = image;
            _height = image.GetLength(0);
            _width = image.GetLength(1);
        }

        public T[,] Constant(int x, int y, int size, T constant = default)
        {
            if (size % 2 != 0) throw new KernelException("Kernel size must be of an odd size.");
            if (x <= _width || _width < 0 || y <= _height || _height < 0)
                throw new KernelException("Kernel must be based of ordinates inside of the image.");

            T[,] kernel = new T[size, size];

            int halfK = size / 2;

            for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                kernel[i, j] = constant;

            int cntY = 0;
            for (int j = y - halfK; j <= y + halfK; j++)
            {
                int cntX = 0;
                for (int i = x - halfK; i <= x + halfK; i++)
                {
                    if (j >= 0 && i >= 0 && j < _image.GetLength(0) && i < _image.GetLength(1))
                    {
                        kernel[cntY, cntX] = _image[j, i];
                    }
                    cntX++;
                }
                cntY++;
            }

            return kernel;
        }

        public T[,] Duplication(int x, int y, int size)
        {
            if (size % 2 != 0) throw new KernelException("Kernel size must be of an odd size.");
            if (x <= _width || _width < 0 || y <= _height || _height < 0)
                throw new KernelException("Kernel must be based of ordinates inside of the image.");

            T[,] kernel = new T[size, size];

            int halfK = size / 2;

            for (int i = 0; i < size; i++) for (int j = 0; j < size; j++) kernel[i, j] = _image[y, x];

            int cntY = 0;
            for (int j = y - halfK; j <= y + halfK; j++)
            {
                int cntX = 0;
                for (int i = x - halfK; i <= x + halfK; i++)
                {
                    if (j >= 0 && i >= 0 && j < _image.GetLength(0) && i < _image.GetLength(1))
                    {
                        kernel[cntY, cntX] = _image[j, i];
                    }
                    cntX++;
                }
                cntY++;
            }

            return kernel;
        }

        public static double[,] Gaussian(double sigma, int size)
        {
            double[,] result = new double[size, size];
            int halfK = size / 2;

            double sum = 0;

            int cntY = -halfK;
            for (int i = 0; i < size; i++)
            {
                int cntX = -halfK;
                for (int j = 0; j < size; j++)
                {
                    result[halfK + cntY, halfK + cntX] = Utility.GaussianDistribution(cntX, cntY, sigma);
                    sum += result[halfK + cntY, halfK + cntX];
                    cntX++;
                }
                cntY++;
            }

            for (int i = 0; i < size; i++) for (int j = 0; j < size; j++) result[i, j] /= sum;
            return result;
        }

    }
}
