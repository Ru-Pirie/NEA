using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib
{
    internal static class Utility
    {
        public static double GaussianDistribution(int x, int y, double sigma) =>
            1 / (2 * Math.PI * sigma * sigma) * Math.Exp(-((Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * sigma * sigma)));
        
        public static double Bound(int l, int h, double v) => v > h ? h : v < l ? l : v;
        
        public static bool TryBound(int l, int h, double v, out double value)
        {
            if (v < h && v > l) value = v;
            else value = v > h ? h : l;
            return v < h && v > l;
        }

        public static double RadianToDegree(double input) => input * 180 / Math.PI;
        public static double DegreeToRadian(double input) => input * Math.PI / 180;
    }
}
