using BackendLib.Exceptions;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace BackendLib.Data
{
    public class Map
    {
        private readonly string _filePath;
        private const string FileExtensionRegex = @"^(\d|\w|(\\)|:|-){1,}.(vmap)$";

        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public bool IsInverted { get; set; }
        public DateTimeOffset TimeCreated { get; set; }
        public Bitmap PathImage { get; set; }
        public Bitmap OriginalImage { get; set; }
        public Bitmap CombinedImage { get; set; }

        public Map() { 
            TimeCreated = DateTimeOffset.Now;
        }

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
                Type = br.ReadInt32();
                IsInverted = br.ReadBoolean();

                int width = (int)br.ReadInt32();
                int height = (int)br.ReadInt32();

                for (int j = 0; j < 3; j++)
                {
                    Structures.RGB[,] tempImage = new Structures.RGB[height, width];
                    for (int i = 0; i < 3; i++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                if (i == 0) tempImage[y, x].R = br.ReadByte();
                                else if (i == 1) tempImage[y, x].G = br.ReadByte();
                                else if (i == 2) tempImage[y, x].B = br.ReadByte();
                            }
                        }
                    }

                    if (j == 0) OriginalImage = tempImage.ToBitmap();
                    else if (j == 1) PathImage = tempImage.ToBitmap();
                    else if (j == 2) CombinedImage = tempImage.ToBitmap();
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
                bw.Write(IsInverted);

                bw.Write((int)OriginalImage.Width);
                bw.Write((int)OriginalImage.Height);

                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int y = 0; y < OriginalImage.Height; y++)
                        {
                            for (int x = 0; x < OriginalImage.Width; x++)
                            {
                                if (j == 0)
                                {
                                    if (i == 0) bw.Write(OriginalImage.GetPixel(x, y).R);
                                    else if(i == 1) bw.Write(OriginalImage.GetPixel(x, y).G);
                                    else if(i == 2) bw.Write(OriginalImage.GetPixel(x, y).B);
                                }
                                else if (j == 1)
                                {
                                    if (i == 0) bw.Write(PathImage.GetPixel(x, y).R);
                                    else if(i == 1) bw.Write(PathImage.GetPixel(x, y).G);
                                    else if(i == 2) bw.Write(PathImage.GetPixel(x, y).B);
                                }
                                else if (j == 2)
                                {
                                    if (i == 0) bw.Write(CombinedImage.GetPixel(x, y).R);
                                    else if (i == 1) bw.Write(CombinedImage.GetPixel(x, y).G);
                                    else if (i == 2) bw.Write(CombinedImage.GetPixel(x, y).B);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
