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
        private T[,] _image;

        public T[,] Constant(int x, int y, int size, double constant = 128)
        {
            if (size % 2 != 0) throw new KernelException("Kernel size must be of an odd size.");
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

        public Matrix Duplication(int x, int y, int size, double constant = 128)
        {
            if (size % 2 != 0) throw new KernelException("Kernel size must be of an odd size.");
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

        public static Matrix Gaussian(double sigma, int size)
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
            return new Matrix(result);
        }

    }
}
