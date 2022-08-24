using System.Drawing;

namespace BackendLib
{
    public class Structures
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

        public struct RawImage
        {
            public Bitmap Original;
            public RGB[,] Pixels;
            public int Width;
            public int Height;
        }
    }
}
