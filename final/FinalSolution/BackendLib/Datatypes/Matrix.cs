using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendLib.Exceptions;

namespace BackendLib.Datatypes
{
    internal class Matrix
    {
        private readonly double[,] _matrix;
        public int X { get; }
        public int Y { get; }

        public Matrix(double[,] matrix)
        {
            _matrix = matrix;
            X = matrix.GetLength(1);
            Y = matrix.GetLength(0);
        }

        public Matrix(int x, int y)
        {
            _matrix = new double[y, x];
            X = x;
            Y = y;
        }


        public double this[int y, int x]
        {
            get => _matrix[y, x];
            private set => _matrix[y, x] = value;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.X != b.X || a.Y != b.Y) throw new MatrixException("Matrices must be the same dimensions to add.");

            Matrix m = new Matrix(a.X, a.Y);
            for (int i = 0; i < a.Y; i++) for (int j = 0; j < a.X; j++) m[i, j] = a[i, j] + b[i, j];
            return m;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.X != b.X || a.Y != b.Y) throw new MatrixException("Matrices must be the same dimensions to subtract.");

            Matrix m = new Matrix(a.X, a.Y);
            for (int i = 0; i < a.Y; i++) for (int j = 0; j < a.X; j++) m[i, j] = a[i, j] - b[i, j];
            return m;
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.X != b.X || a.Y != b.Y) throw new MatrixException("Matrices must be the same dimensions to multiply.");

            Matrix m = new Matrix(a.X, a.Y);
            for (int i = 0; i < a.Y; i++) for (int j = 0; j < a.X; j++) m[i, j] = a[i, j] * b[i, j];
            return m;
        }

        public static Matrix operator *(int a, Matrix b)
        {
            Matrix m = new Matrix(b.X, b.Y);
            for (int i = 0; i < b.Y; i++) for (int j = 0; j < b.X; j++) m[i, j] = a * b[i, j];
            return m;
        }

        public void Minimize()
        {
            double sum = 0;
            foreach (double val in _matrix) sum += val;

            for (int i = 0; i < Y; i++)
            {
                for (int j = 0; j < X; j++)
                {
                    _matrix[i, j] /= sum;
                }
            }
        }

        public static double Convolution(Matrix a, Matrix b)
        {
            if (a.X != b.X || b.Y != a.Y) throw new MatrixException("Matrices must be the same dimensions to apply convolution.");

            double[,] flippedB = new double[b.Y, b.X];
            int l = b.X;
            for (int i = l - 1; i >= 0; i--) for (int j = l - 1; j >= 0; j--) flippedB[b.Y - (i + 1), b.X - (j + 1)] = b[i, j];

            double sum = 0;
            for (int i = 0; i < a.Y; i++) for (int j = 0; j < a.X; j++) sum += a[i, j] * flippedB[i, j];
            return sum;
        }
    }
}
