using System.Drawing;
using System.IO;
using BackendLib.Exceptions;

namespace BackendLib.Processing
{
    internal class Pre
    {
        private readonly string _imagePath;
        private Bitmap _imageBitmap;
        private Structures.Rgb[,] _imageRgb;

        private readonly string _fileExtentionRegex = @"^(\d|\w|(\\)|:){1,}.(jpg|bmp|exif|png|tiff)$";

        public Pre(string imagePath)
        {
            _imagePath = imagePath;
        }

        public void Start()
        {

        }

        private void ValidatePath()
        {
            if (!File.Exists(_imagePath)) throw PreprocessingException("Supplied Image Does Not Exist");
        }


        public Structures.Rgb[,] Result() => _imageRgb;

    }
}
