
using System;
using System.Threading.Tasks;
using BackendLib.Datatypes;

namespace BackendLib.Processing
{
    public class CannyEdgeDetection
    {
        public int KernelSize { get; set; } = 5;
        public double RedRatio { get; set; } = 0.299;
        public double GreenRatio { get; set; } = 0.587;
        public double BlueRatio { get; set; } = 0.114;
        public double Sigma { get; set; } = 1.4;
        public double LowerThreshold { get; set; } = 0.1;
        public double UpperThreshold { get; set; } = 0.3;

        public CannyEdgeDetection() { }

        public CannyEdgeDetection(int kernelSize, double redRatio, double greenRatio, double blueRatio, double sigma, double lowerThreshold, double upperThreshold)
        {
            KernelSize = kernelSize;
            RedRatio = redRatio;
            GreenRatio = greenRatio;
            BlueRatio = blueRatio;
            Sigma = sigma;
            LowerThreshold = lowerThreshold;
            UpperThreshold = upperThreshold;
        }




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
                    output[y, x] = (input[y, x].R * RedRatio) + (input[y, x].G * GreenRatio) + (input[y, x].B * BlueRatio);
                }
            }

            return output;
        }
        
        public double[,] GaussianFilter(double[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            Matrix gaussianKernel = new Matrix(Kernel<double>.Gaussian(Sigma, KernelSize));
            Kernel<double> masterKernel = new Kernel<double>(input);

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    Matrix subKernel = new Matrix(masterKernel.Duplication(x, y, KernelSize));
                    double sum = Matrix.Convolution(subKernel, gaussianKernel);
                    output[y, x] = sum;
                }
            }

            return output;
        }

        public Structures.Gradients CalculateGradients(double[,] input, Action updateMenu)
        {
            Task<double[,]>[] tasks =
            {
                new Task<double[,]>(() => CalculateGradientX(input, updateMenu)),
                new Task<double[,]>(() => CalculateGradientY(input, updateMenu))
            };

            foreach (var task in tasks) task.Start();
            
            Task.WaitAll(tasks);

            return new Structures.Gradients
            {
                GradientX = tasks[0].Result, 
                GradientY = tasks[1].Result
            };
        }

        private double[,] CalculateGradientX(double[,] input, Action updateMenu)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            Matrix sobelMatrixY = new Matrix(new double[,] { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } });
            Kernel<double> masterKernel = new Kernel<double>(input);

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    Matrix imageKernel = new Matrix(masterKernel.Duplication(x, y, KernelSize));
                    output[y, x] = Matrix.Convolution(imageKernel, sobelMatrixY);
                }
            }

            updateMenu();
            return output;
        }

        private double[,] CalculateGradientY(double[,] input, Action updateMenu)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            Matrix sobelMatrixY = new Matrix(new double[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } });
            Kernel<double> masterKernel = new Kernel<double>(input);

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    Matrix imageKernel = new Matrix(masterKernel.Duplication(x, y, KernelSize));
                    output[y, x] = Matrix.Convolution(imageKernel, sobelMatrixY);
                }
            }

            updateMenu();
            return output;
        }

        public double[,] CombineGradients(Structures.Gradients grads)
        {
            if (grads.GradientX.GetLength(0) != grads.GradientY.GetLength(0) || grads.GradientX.GetLength(1) != grads.GradientY.GetLength(1))
                throw new ArgumentException("Supplied arrays where not of same dimensions");

            double[,] output = new double[grads.GradientX.GetLength(0), grads.GradientX.GetLength(1)];

            for (int y = 0; y < grads.GradientX.GetLength(0); y++)
            {
                for (int x = 0; x < grads.GradientX.GetLength(1); x++)
                {
                    output[y, x] = Math.Sqrt(Math.Pow(grads.GradientX[y, x], 2) + Math.Pow(grads.GradientY[y, x], 2));
                }
            }
            
            return output;
        }

        public double[,] GradientAngle(Structures.Gradients grads)
        {
            if (grads.GradientX.GetLength(0) != grads.GradientY.GetLength(0) || grads.GradientX.GetLength(1) != grads.GradientY.GetLength(1))
                throw new ArgumentException("Supplied arrays where not of same dimensions");

            double[,] output = new double[grads.GradientX.GetLength(0), grads.GradientX.GetLength(1)];

            for (int y = 0; y < grads.GradientX.GetLength(0); y++)
            {
                for (int x = 0; x < grads.GradientX.GetLength(1); x++)
                {
                    output[y, x] = Math.Atan2(grads.GradientY[y, x], grads.GradientX[y, x]);
                }
            }

            return output;
        }

        public double[,] MagnitudeThreshold(double[,] gradCombined, double[,] gradAngle)
        {
            if (gradCombined.GetLength(0) != gradAngle.GetLength(0) || gradCombined.GetLength(1) != gradAngle.GetLength(1)) 
                throw new ArgumentException("Supplied arrays where not of same dimensions");

            double[,] output = gradCombined;
            double[,] anglesInDegrees = new double[gradCombined.GetLength(0), gradCombined.GetLength(1)];

            for (int y = 0; y < anglesInDegrees.GetLength(0); y++)
            {
                for (int x = 0; x < anglesInDegrees.GetLength(1); x++)
                {
                    anglesInDegrees[y, x] = Utility.RadianToDegree(gradAngle[y, x]);
                }
            }

            Kernel<double> masterKernel = new Kernel<double>(gradCombined);

            for(int y = 0; y < anglesInDegrees.GetLength(0); y++)
            {
                for (int x = 0; x < anglesInDegrees.GetLength(1); x++)
                {
                    double[,] magnitudeKernel = masterKernel.Duplication(x, y, 3);

                    if (anglesInDegrees[y, x] < 22.5 || anglesInDegrees[y, x] >= 157.5)
                    {
                        if (gradCombined[y, x] < magnitudeKernel[1, 2] || gradCombined[y, x] < magnitudeKernel[1, 0])
                            output[y, x] = 0;
                    }
                    else if (anglesInDegrees[y, x] >= 22.5 && anglesInDegrees[y, x] < 67.5)
                    {
                        if (gradCombined[y, x] < magnitudeKernel[0, 2] || gradCombined[y, x] < magnitudeKernel[2, 0])
                            output[y, x] = 0;
                    }
                    else if (anglesInDegrees[y, x] >= 67.5 && anglesInDegrees[y, x] < 112.5)
                    {
                        if (gradCombined[y, x] < magnitudeKernel[0, 1] || gradCombined[y, x] < magnitudeKernel[2, 1])
                            output[y, x] = 0;
                    }
                    else if (anglesInDegrees[y, x] >= 112.5 && anglesInDegrees[y, x] < 157.5)
                    {
                        if (gradCombined[y, x] < magnitudeKernel[0, 0] || gradCombined[y, x] < magnitudeKernel[2, 2])
                            output[y, x] = 0;
                    }
                    else throw new Exception();
                }
            }

            return output;
        }

        public Structures.ThresholdPixel[,] DoubleThreshold(double[,] input)
        {
            double min = LowerThreshold * 255;
            double max = UpperThreshold * 255;

            Structures.ThresholdPixel[,] output = new Structures.ThresholdPixel[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    if (input[y, x] < min) output[y, x] = new Structures.ThresholdPixel{Strong =  false, Value = 0};
                    else if (input[y, x] > min && input[y, x] < max) output[y, x] = new Structures.ThresholdPixel { Strong = false, Value = input[y, x] };
                    else if (input[y, x] > max) output[y, x] = new Structures.ThresholdPixel { Strong = true, Value = input[y, x] };
                    else throw new Exception();
                }
            }

            return output;
        }

        public double[,] EdgeTrackingHysteresis(Structures.ThresholdPixel[,] input)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            Kernel<Structures.ThresholdPixel> masterKernel = new Kernel<Structures.ThresholdPixel>(input);

            for (int i = 0; i < input.GetLength(0); i++)
            {
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    if (input[i, j].Strong == false)
                    {
                        Structures.ThresholdPixel[,] imageKernel = masterKernel.Duplication(j, i, 3);
                        bool strong = false;
                        for (int k = 0; k < 3 && !strong; k++)
                        {
                            for (int l = 0; l < 3 && !strong; l++)
                            {
                                if (imageKernel[k, l].Strong) strong = true;
                            }
                        }
                        output[i, j] = strong ? 255 : 0;
                    }
                    else output[i, j] = 255;
                }
            }

            return output;
        }
    }
}