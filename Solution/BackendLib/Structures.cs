using BackendLib.Data;
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
            public string Path;
            public RGB[,] Pixels;
            public int Width;
            public int Height;
            public MapFile MapFile;
        }

        public struct RoadResult
        {
            public Bitmap FilledBitmap;
            public Bitmap PathBitmap;
            public double[,] PathDoubles;
        }

        public struct CannyResult
        {
            public Bitmap BitmapImage;
            public double[,] DoubleImage;
        }

        public struct Coord
        {
            public int X;
            public int Y;

            public override string ToString() => $"({X}, {Y})";
            public bool Equals(Coord other) => X == other.X && Y == other.Y;
            public override bool Equals(object obj) => obj is Coord other && Equals(other);
            public static bool operator ==(Coord lhs, Coord rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;
            public static bool operator !=(Coord lhs, Coord rhs) => !(lhs == rhs);
            public override int GetHashCode()
            {
                unchecked
                {
                    return (X * 397) ^ Y;
                }
            }
        }
    }
}