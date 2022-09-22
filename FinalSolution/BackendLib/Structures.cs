using System.Drawing;
using System.Security.Policy;

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

        public struct RoadResult
        {
            public Bitmap FilledBitmap;
            public Bitmap PathBitmap;
        }

        public struct CannyResult
        {
            public Bitmap BitmapImage;
            public double[,] DoubleImage;
        }

        public struct Cord
        {
            public int X;
            public int Y;
        }
    }
}
