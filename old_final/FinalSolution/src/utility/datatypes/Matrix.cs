using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSolution.src.utility.datatypes
{
    public class Matrix
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public double[,] matrix { get; private set; }

        public Matrix(int X, int Y)
        {
            this.X = X;
            this.Y = Y;

            matrix = new double[Y, X];
        }

        public Matrix(double[,] matrix)
        {
            Y = matrix.GetLength(0);
            X = matrix.GetLength(1);

            this.matrix = matrix;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.X != b.X || a.Y != b.Y) throw new MatrixException("Matrices must be the same dimensions to add.");

            Matrix m = new Matrix(a.X, a.Y);
            for (int i = 0; i < a.Y; i++) for (int j = 0; j < a.X; j++) m.matrix[i, j] = a.matrix[i, j] + b.matrix[i, j];

            return m;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.X != b.X || a.Y != b.Y) throw new MatrixException("Matrices must be the same dimensions to subtract.");

            Matrix m = new Matrix(a.X, a.Y);
            for (int i = 0; i < a.Y; i++) for (int j = 0; j < a.X; j++) m.matrix[i, j] = a.matrix[i, j] - b.matrix[i, j];
            
            return m;
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.X != b.X || a.Y != b.Y) throw new MatrixException("Matrices must be the same dimensions to multiply.");

            Matrix m = new Matrix(a.X, a.Y);
            for (int i = 0; i < a.Y; i++) for (int j = 0; j < a.X; j++) m.matrix[i, j] = a.matrix[i, j] * b.matrix[i, j];

            return m;
        }

        public static Matrix operator *(int a, Matrix b)
        {
            Matrix m = new Matrix(b.X, b.Y);
            for (int i = 0; i < b.Y; i++) for (int j = 0; j < b.X; j++) m.matrix[i, j] = a * b.matrix[i, j];

            return m;
        }

        public static Matrix Minimise(Matrix a)
        {
            double sum = 0;
            foreach (double val in a.matrix) sum += val;

            for (int i = 0; i < a.Y; i++)
            {
                for (int j = 0; j < a.X; j++)
                {
                    a.matrix[i, j] /= sum;
                }
            }

            return a;
        }

        public static double Convolution(Matrix a, Matrix b)
        {
            if (a.X != b.X || b.Y != a.Y) throw new MatrixException("Matrices must be the same dimensions to apply convolution.");

            double[,] flippedB = new double[b.Y, b.X];
            int l = b.X;
            for (int i = l - 1; i >= 0; i--)
            {
                for (int j = l - 1; j >= 0; j--)
                {
                    flippedB[b.Y - (i + 1), b.X - (j + 1)] = b.matrix[i, j];
                }
            }

            double sum = 0;
            for (int i = 0; i < a.Y; i++)
            {
                for (int j = 0; j < a.X; j++)
                {
                    sum += a.matrix[i, j] * flippedB[i, j];
                }
            }

            return sum;
        }
    }
}