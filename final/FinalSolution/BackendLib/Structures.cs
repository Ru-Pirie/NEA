using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib
{
    internal class Structures
    {
        public struct ThresholdPixel
        {
            public bool Strong;
            public double Value;
        }

        public struct RGB
        {
            public double R;
            public double G;
            public double B;
        }

        public struct Gradients
        {
            public double[,] GradientX;
            public double[,] GradientY;
        }
    }
}
