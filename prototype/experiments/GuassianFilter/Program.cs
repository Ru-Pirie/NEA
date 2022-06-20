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
                        Math.Exp(-((Math.Pow(i - (k + 1), 2) + Math.Pow(j - (k + 1), 2)) / (2 * sigma * sigma)));
                }
            }



        }
    }
}
