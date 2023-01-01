using System;
using System.Collections.Generic;
using System.Text;

namespace ColourConversion
{
    static class Colour
    {
        // Observer= 2°, Illuminant= D65
        private const double _refX = 95.0489;
        private const double _refY = 100.000;
        private const double _refZ = 108.8840;
        private const double _epsilon = 216 / 24389;
        private const double _kappa = 24389 / 27;
        public struct RGB
        {
            public double R; public double G; public double B;

            public static RGB operator +(RGB a, RGB b) => new RGB { R = (a.R + b.R) / 2, G = (a.G + b.G) / 2, B = (a.B + b.B) / 2 };
        };
        public struct YIQ
        {
            public double Y; public double I; public double Q;

            public static YIQ operator +(YIQ a, YIQ b) => new YIQ { Y = (a.Y + b.Y) / 2, I = (a.I + b.I) / 2, Q = (a.Q + b.Q) / 2 };
        };

        public struct XYZ
        {
            public double X; public double Y; public double Z;

            public static XYZ operator +(XYZ a, XYZ b) => new XYZ { X = (a.X + b.X) / 2, Y = (a.Y + b.Y) / 2, Z = (a.Z + b.Z) / 2 };
        }

        public struct LAB
        {
            public double L; public double A; public double B;

            public static LAB operator +(LAB a, LAB b) => new LAB { L = (a.L + b.L) / 2, A = (a.A + b.A) / 2, B = (a.B + b.B) / 2 };
        }

        public struct LUV
        {
            public double L; public double U; public double V;

            public static LUV operator +(LUV a, LUV b) => new LUV { L = (a.L + b.L) / 2, U = (a.U + b.U) / 2, V = (a.V + b.V) / 2 };
        }

        public struct HSL
        {
            public double H; public double S; public double L;

            public static HSL operator +(HSL a, HSL b) => new HSL { H = (a.H + b.H) / 2, S = (a.S + b.S) / 2, L = (a.L + b.L) / 2 };
        }

        public struct HSV
        {
            public double H; public double S; public double V;

            public static HSV operator +(HSV a, HSV b) => new HSV { H = (a.H + b.H) / 2, S = (a.S + b.S) / 2, V = (a.V + b.V) / 2 };
        }

        private static void DescaleRGB(ref RGB pixel)
        {
            pixel.R /= 255;
            pixel.G /= 255;
            pixel.B /= 255;
        }

        private static void RescaleRGB(ref RGB pixel)
        {
            pixel.R *= 255;
            pixel.G *= 255;
            pixel.B *= 255;
        }

        private static void LinearizeRGB(ref RGB pixel)
        {
            DescaleRGB(ref pixel);

            pixel.R = pixel.R <= 0.04045 ? pixel.R / 12.92 : Math.Pow((pixel.R + 0.055) / 1.055, 2.4);
            pixel.G = pixel.G <= 0.04045 ? pixel.G / 12.92 : Math.Pow((pixel.G + 0.055) / 1.055, 2.4);
            pixel.B = pixel.B <= 0.04045 ? pixel.B / 12.92 : Math.Pow((pixel.B + 0.055) / 1.055, 2.4);
        }

        private static void CompandRGB(ref RGB pixel)
        {
            pixel.R = pixel.R <= 0.0031308 ? 12.92 * pixel.R : (1.055 * Math.Pow(pixel.R, 1.0 / 2.4)) - 0.055;
            pixel.G = pixel.G <= 0.0031308 ? 12.92 * pixel.G : (1.055 * Math.Pow(pixel.G, 1.0 / 2.4)) - 0.055;
            pixel.B = pixel.B <= 0.0031308 ? 12.92 * pixel.B : (1.055 * Math.Pow(pixel.B, 1.0 / 2.4)) - 0.055;

            RescaleRGB(ref pixel);
        }

        public static HSV HSLToHSV(HSL pixel)
        {
            pixel.S /= 100;
            pixel.L /= 100;

            double min = pixel.L > 1 - pixel.L ? 1 - pixel.L : pixel.L;
            double lambda = pixel.L + (pixel.S * min);

            return new HSV
            {
                H = pixel.H,
                S = lambda == 0 ? 0 : 200 * (1 - (pixel.L / lambda)),
                V = lambda * 100
            };
        }
        public static HSL RGBToHSL(RGB pixel)
        {
            DescaleRGB(ref pixel);

            double max = pixel.R > pixel.G && pixel.R > pixel.B ? pixel.R : (pixel.G > pixel.B ? pixel.G : pixel.B);
            double min = pixel.R > pixel.G && pixel.R > pixel.B ? (pixel.G > pixel.B ? pixel.B : pixel.G) : pixel.R;

            double delta = max - min;

            double H =
                pixel.R == max ? (pixel.G - pixel.B) / delta :
                pixel.G == max ? 2 + (pixel.B - pixel.R) / delta :
                pixel.B == max ? 4 + (pixel.R - pixel.G) / delta :
                0;

            H = (H * 60) < 0.0 ? (H * 60) + 360 : H * 60;

            double S = 0;
            if (max != min)
            {
                double alpha = (max + min) / 2;
                S = alpha <= 0.5 ? (max - min) / (max + min) : (max - min) / (2 - max - min);
            }

            return new HSL
            {
                H = pixel.R == pixel.G && pixel.G == pixel.B ? 0 : H,
                S = S * 100,
                L = (max + min) / 2 * 100
            };
        }

        public static RGB HSVToRGB(HSV pixel)
        {
            RGB outPixel = new RGB();

            if (pixel.S == 0)
            {
                outPixel.R = pixel.V;
                outPixel.G = pixel.V;
                outPixel.B = pixel.V;
            }

            double delta = pixel.H >= 360 ? 0 : pixel.H / 60;
            long alpha = (long)delta;
            double beta = delta - alpha;

            double chi = pixel.V * (1.0 - pixel.S);
            double psi = pixel.V * (1.0 - (pixel.S * beta));
            double phi = pixel.V * (1.0 - (pixel.S * (1.0 - beta)));

            switch (alpha)
            {
                case 0:
                    outPixel.R = pixel.V;
                    outPixel.G = phi;
                    outPixel.B = chi;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                default:
                    break;
            }

            return new RGB
            {
                R = outPixel.R * 255,
                G = outPixel.G * 255,
                B = outPixel.B * 255
            };
        }

        public static XYZ RGBToXYZ(RGB pixel)
        {
            LinearizeRGB(ref pixel);

            return new XYZ
            {
                X = ((0.4124564 * pixel.R) + (0.3575761 * pixel.G) + (0.1804375 * pixel.B)) * 100,
                Y = ((0.2126729 * pixel.R) + (0.7151522 * pixel.G) + (0.0721750 * pixel.B)) * 100,
                Z = ((0.0193339 * pixel.R) + (0.1191920 * pixel.G) + (0.9503041 * pixel.B)) * 100
            };
        }

        public static RGB XYZToRGB(XYZ pixel)
        {
            pixel.X /= 100;
            pixel.Y /= 100;
            pixel.Z /= 100;

            RGB convertedPixel = new RGB
            {
                R = (3.2404542 * pixel.X) - (1.5371385 * pixel.Y) - (0.4985314 * pixel.Z),
                G = -(0.9692660 * pixel.X) + (1.8760108 * pixel.Y) + (0.0415560 * pixel.Z),
                B = (0.0556435 * pixel.X) - (0.2040259 * pixel.Y) + (1.0572252 * pixel.Z)
            };

            CompandRGB(ref convertedPixel);
            return convertedPixel;
        }

        public static LAB XYZToLAB(XYZ pixel)
        {
            double chiX = pixel.X / _refX;
            double psiY = pixel.Y / _refY;
            double phiZ = pixel.Z / _refZ;

            double funcX = chiX > _epsilon ? Math.Pow(chiX, 1.0 / 3.0) : ((_kappa * chiX) + 16) / 116.0;
            double funcY = psiY > _epsilon ? Math.Pow(psiY, 1.0 / 3.0) : ((_kappa * psiY) + 16) / 116.0;
            double funcZ = phiZ > _epsilon ? Math.Pow(phiZ, 1.0 / 3.0) : ((_kappa * phiZ) + 16) / 116.0;

            double L = (116.0 * funcY) - 16;
            double A = 500.0 * (funcX - funcY);
            double B = 200.0 * (funcY - funcZ);

            return new LAB { L = L, A = A, B = B };
        }

        public static LUV XYZToLUV(XYZ pixel)
        {
            double internalY = pixel.Y / _refY;

            double mue = 4 * pixel.X / (pixel.X + (15 * pixel.Y) + (3 * pixel.Z));
            double lambda = 9 * pixel.Y / (pixel.X + (15 * pixel.Y) + (3 * pixel.Z));

            double muePrime = 4 * _refX / (_refX + (15 * _refY) + (3 * _refZ));
            double lambdaPrime = 9 * _refY / (_refX + (15 * _refY) + (3 * _refZ));

            double L = internalY > _epsilon ? (116.0 * Math.Pow(internalY, 1.0 / 3.0)) - 16 : _kappa * internalY;
            double U = 13 * L * (mue - muePrime);
            double V = 13 * L * (lambda - lambdaPrime);

            return new LUV { L = L, U = U, V = V };
        }

        public static XYZ LABToXYZ(LAB pixel)
        {
            double psiY = (pixel.L + 16) / 116.0;
            double chiX = pixel.A / 500.0 + psiY;
            double phiZ = psiY - pixel.B / 200.0;

            double X = Math.Pow(chiX, 3) > 0.008856 ? Math.Pow(chiX, 3) : (chiX - 16.0 / 116.0) / 7.787;
            double Y = Math.Pow(psiY, 3) > 0.008856 ? Math.Pow(psiY, 3) : (psiY - 16.0 / 116.0) / 7.787;
            double Z = Math.Pow(phiZ, 3) > 0.008856 ? Math.Pow(phiZ, 3) : (phiZ - 16.0 / 116.0) / 7.787;

            return new XYZ
            {
                X = X * _refX,
                Y = Y * _refY,
                Z = Z * _refZ
            };
        }

        public static XYZ LUVToXYZ(LUV pixel)
        {
            double mue = 4 * _refX / (_refX + (15 * _refY) + (3 * _refZ));
            double lambda = 9 * _refY / (_refX + (15 * _refY) + (3 * _refZ));

            double Y = pixel.L > _kappa * _epsilon ? Math.Pow((pixel.L + 16) / 116, 3) : pixel.L / _kappa;

            double alpha = (1.0 / 3.0) * (((52 * pixel.L) / (pixel.U + (13 * pixel.L * mue))) - 1);
            double beta = (-5.0) * Y;
            double gamma = -1.0 / 3.0;
            double delta = (Y) * (((39 * pixel.L) / (pixel.V + (13 * pixel.L * lambda))) - 5);

            double X = (delta - beta) / (alpha - gamma);
            double Z = (X * alpha) + beta;

            return new XYZ
            {
                X = X * 100,
                Y = Y * 100,
                Z = Z * 100
            };
        }
    }
}
