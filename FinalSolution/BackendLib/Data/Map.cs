using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BackendLib.Exceptions;

namespace BackendLib.Data
{
    public class Map
    {
        private readonly string _filePath;
        private const string FileExtensionRegex = @"^(\d|\w|(\\)|:|-){1,}.(vmap)$";

        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTimeOffset TimeCreated { get; set; }
        public Bitmap PathImage { get; set; }
        public Bitmap OriginalImage { get; set; }
        public Bitmap CombinedImage { get; set; }

        public Map() {}

        public Map(string filePath)
        {
            _filePath = filePath;
        }

        public void Initialize()
        {
            ValidateImage();
            ReadMapFile();
        }

        private void ValidateImage()
        {
            Regex fileRegex = new Regex(FileExtensionRegex, RegexOptions.IgnoreCase);

            if (!File.Exists(_filePath)) throw new MapException("Supplied Virtual Map File Does Not Exist");
            if (!fileRegex.IsMatch(_filePath)) throw new MapException("Supplied Virtual Map File Is Not Of Valid Type. (vmap)");
        } 

        private void ReadMapFile()
        {
            using (BinaryReader br = new BinaryReader(File.Open(_filePath, FileMode.Open)))
            {
                string dateTime = br.ReadString();
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(double.Parse(dateTime)).ToLocalTime();
                TimeCreated = new DateTimeOffset(dt);

                Name = br.ReadString();
                Description = br.ReadString();
                Type = br.ReadString();

                int width = br.ReadInt32();
                int height = br.ReadInt32();

                for (int i = 0; i < 3; i++)
                {
                    double[,] tempDoubles = new double[height, width]; 

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            tempDoubles[y, x] = br.ReadDouble();
                        }
                    }

                    if (i == 0) OriginalImage = tempDoubles.ToBitmap();
                    else if (i == 1) PathImage = tempDoubles.ToBitmap();
                    else if (i == 2) CombinedImage = tempDoubles.ToBitmap();
                }
            }
        }

        public void Save(Guid currentGuid)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open($"./saves/{currentGuid}.vmap", FileMode.OpenOrCreate)))
            {
                bw.Write(TimeCreated.ToUnixTimeMilliseconds().ToString());

                bw.Write(Name);
                bw.Write(Description);
                bw.Write(Type);

                bw.Write(OriginalImage.Width);
                bw.Write(OriginalImage.Height);

                for (int i = 0; i < 3; i++)
                {
                    for (int y = 0; y < OriginalImage.Height; y++)
                    {
                        for (int x = 0; x < OriginalImage.Width; x++)
                        {
                           if (i == 0) bw.Write((double)OriginalImage.GetPixel(x, y).R);
                           else if (i == 1) bw.Write((double)PathImage.GetPixel(x, y).R);
                           else if (i == 2) bw.Write((double)CombinedImage.GetPixel(x, y).R);
                        }
                    }
                }
            }
        }

    }
}
