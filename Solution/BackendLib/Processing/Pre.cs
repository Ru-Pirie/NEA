﻿using BackendLib.Exceptions;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace BackendLib.Processing
{
    public class Pre
    {
        private readonly string _imagePath;
        private Bitmap _imageBitmap;
        private Structures.RGB[,] _imageRgb;

        private const string FileExtensionRegex = @"^([a-z]:\\|\\|[a-z]|\.\.(\\|\/)|\.(\\|\/))((\w|(\\|\/))+)\.(jpg|bmp|exif|png|tiff)$";

        public Pre(string imagePath)
        {
            _imagePath = imagePath;
        }


        /// <exception cref="PreprocessingException"></exception>
        /// <exception cref="Exception"></exception>
        public void Start(Action updateProgressAction)
        {
            updateProgressAction();
            ValidatePath();
            updateProgressAction();
            ReadImage();
            updateProgressAction();
            CheckDimensions();
            updateProgressAction();
        }

        private void ValidatePath()
        {
            Regex fileRegex = new Regex(FileExtensionRegex, RegexOptions.IgnoreCase);

            if (!File.Exists(_imagePath)) throw new PreprocessingException("The image that you entered does not exist, double check the path to the file and that exists.");
            if (!fileRegex.IsMatch(_imagePath)) throw new PreprocessingException("The file which you entered does not appear to be an image file. It should end in .jpg, .bmp, .exif, .png or .tiff double check and try again.");
        }

        private void ReadImage()
        {
            _imageBitmap = new Bitmap(_imagePath, true);
            _imageRgb = new Structures.RGB[_imageBitmap.Height, _imageBitmap.Width];

            for (int y = 0; y < _imageBitmap.Height; y++)
            {
                for (int x = 0; x < _imageBitmap.Width; x++)
                {
                    Color tempPixel = _imageBitmap.GetPixel(x, y);
                    _imageRgb[y, x] = new Structures.RGB
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
            if (_imageRgb.GetLength(0) < 200 || _imageRgb.GetLength(1) < 200)
                throw new PreprocessingException("The image you supplied is too small to work properly it must be at least 200x200. Try a larger image.");

            if (_imageRgb.GetLength(0) % 2 != 0 || _imageRgb.GetLength(1) % 2 != 0)
            {
                Structures.RGB[,] resizedRgb =
                    new Structures.RGB[_imageRgb.GetLength(0) / 2 * 2, _imageRgb.GetLength(1) / 2 * 2];

                for (int y = 0; y < _imageRgb.GetLength(0) / 2 * 2; y++)
                {
                    for (int x = 0; x < _imageRgb.GetLength(1) / 2 * 2; x++)
                    {
                        resizedRgb[y, x] = _imageRgb[y, x];
                    }
                }

                _imageRgb = resizedRgb;
            }
        }

        public Structures.RawImage Result() => new Structures.RawImage
        {
            Original = _imageBitmap,
            Pixels = _imageRgb,
            Path = _imagePath,
            Height = _imageBitmap.Height,
            Width = _imageBitmap.Width
        };

    }
}