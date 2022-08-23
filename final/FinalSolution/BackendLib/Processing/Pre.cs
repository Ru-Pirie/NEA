using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using BackendLib.Exceptions;

namespace BackendLib.Processing
{
    public class Pre
    {
        private readonly string _imagePath;
        private Bitmap _imageBitmap;
        private Structures.RGB[,] _imageRGB;

        private readonly string _fileExtentionRegex = @"^(\d|\w|(\\)|:){1,}.(jpg|bmp|exif|png|tiff)$";

        public Pre(string imagePath)
        {
            _imagePath = imagePath;
        }

        public void Start(Action logMessage)
        {
            ValidatePath();
            ReadImage();
            CheckDimensions();
        }

        private void ValidatePath()
        {
            if (!File.Exists(_imagePath)) throw new PreprocessingException("Supplied Image Does Not Exist");
            if (!Regex.IsMatch(_imagePath, _fileExtentionRegex)) throw new PreprocessingException("Supplied Image File Is Not Of Valid Type. (jpg, bmp, exif, png, tiff)");
        }

        private void ReadImage()
        {
            _imageBitmap = new Bitmap(_imagePath, true);
            _imageRGB = new Structures.RGB[_imageBitmap.Height, _imageBitmap.Width];

            for (int y = 0; y < _imageBitmap.Height; y++)
            {
                for (int x = 0; x < _imageBitmap.Width; x++)
                {
                    Color tempPixel = _imageBitmap.GetPixel(x, y);
                    _imageRGB[y, x] = new Structures.RGB
                    {
                        R = tempPixel.R,
                        G = tempPixel.G,
                        B = tempPixel.B
                    };
                }
            }
        }

        private void CheckDimensions()
        {
            if (_imageRGB.GetLength(0) < 200 || _imageRGB.GetLength(1) < 200)
                throw new PreprocessingException("Supplied Image Is Too Small To Be Processed");

            if (_imageRGB.GetLength(0) % 2 != 0 || _imageRGB.GetLength(1) % 2 != 0)
            {
                Structures.RGB[,] resizedRGB =
                    new Structures.RGB[_imageRGB.GetLength(0) / 2 * 2, _imageRGB.GetLength(1) / 2 * 2];

                for (int y = 0; y < _imageRGB.GetLength(0) / 2 * 2; y++)
                {
                    for (int x = 0; x < _imageRGB.GetLength(1) / 2 * 2; x++)
                    {
                        resizedRGB[y, x] = _imageRGB[y, x];
                    }    
                }

                _imageRGB = resizedRGB;
            }
        }

        public Bitmap Original() => _imageBitmap;
        public Structures.RawImage Result() => new Structures.RawImage
        {
            Original = _imageBitmap, 
            Pixels = _imageRGB, 
            Height = _imageBitmap.Height, 
            Width = _imageBitmap.Width
        };

    }
}
