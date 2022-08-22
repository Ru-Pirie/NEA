using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Processing
{
    internal class CannyEdgeDetection
    {
        public int _kernelSize = 5;
        public double _redRatio = 0.299, _greenRatio = 0.587, _blueRatio = 0.114, _sigma = 1.4, _lowerThreshold = 0.1, _upperThreshold = 0.3;

        /// <summary>
        /// Convert a given image in the form of a RGB double array will convert it to a single double array of black and white pixels.
        /// </summary>
        /// <param name="input">The image to be converted to black and white</param>
        /// <returns>The processed double array</returns>
        public double[,] BlackWhiteFilter(Structures.RGB[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    output[y, x] = (input[y, x].R * _redRatio) + (input[y, x].G * _greenRatio) + (input[y, x].B * _blueRatio);
                }
            }

            return output;
        }


        public double[,] GaussianFilter(double[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];



            
            return new double[1, 1];
        }

        public double[,] CalculateGradients(double[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            
            return new double[1, 1];
        }

        private double[,] CalculateGradientX(double[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            
            return new double[1, 1];
        }

        private double[,] CalculateGradientY(double[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            
            return new double[1, 1];
        }

        public double[,] CombineGradients(double[,] gradX, double[,] gradY)
        {
            if (gradX.GetLength(0) != gradY.GetLength(0) || gradX.GetLength(1) != gradY.GetLength(1))
                throw new ArgumentException("Supplied arrays where not of same dimensions");

            double[,] output = new double[gradX.GetLength(0), gradX.GetLength(1)];

            
            return new double[1, 1];
        }

        public double[,] GradientAngle(double[,] gradX, double[,] gradY)
        {
            if (gradX.GetLength(0) != gradY.GetLength(0) || gradX.GetLength(1) != gradY.GetLength(1))
                throw new ArgumentException("Supplied arrays where not of same dimensions");

            double[,] output = new double[gradX.GetLength(0), gradX.GetLength(1)];

            
            return new double[1, 1];
        }

        public double[,] MagnitudeThreshold(double[,] gradCombined, double[,] gradAngle)
        {
            if (gradCombined.GetLength(0) != gradAngle.GetLength(0) || gradCombined.GetLength(1) != gradAngle.GetLength(1)) 
                throw new ArgumentException("Supplied arrays where not of same dimensions");

            double[,] output = new double[gradCombined.GetLength(0), gradCombined.GetLength(1)];

            
            return new double[1, 1];
        }

        public Structures.ThresholdPixel[,] DoubleThreshold(double[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            
            return new Structures.ThresholdPixel[1, 1];
        }

        public double[,] EdgeTrackingHysteresis(Structures.ThresholdPixel[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            
            return new double[1, 1];
        }
    }
}
