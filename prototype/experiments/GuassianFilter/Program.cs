using System;

namespace GuassianFilter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double sigma = 1;
            int k = 5;

            double[,] kernel = new double[k, k];
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    kernel[i, j] = 1 / (2 * Math.PI * sigma * sigma) *
                        Math.Exp(-((Math.Pow(i, 2) + Math.Pow(j, 2)) / (2 * sigma * sigma)));
                }
            }

            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    Console.Write(Math.Round(kernel[i, j], 2)+ " ");
                }
                Console.WriteLine();
            }

        }
    }
}
