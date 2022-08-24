using System;
using System.Drawing;
using System.IO;
using BackendLib.Exceptions;

namespace BackendLib
{
    public class Logger
    {
        private readonly bool _localApplication;

        public Logger(bool local)
        {
            _localApplication = local;
            CreateDirStructure();
        }

        private void CreateDirStructure()
        {
            Directory.CreateDirectory("./run");
            Directory.CreateDirectory("./logs");

            string mode = _localApplication ? "Local Application" : "Web Application";
            using (StreamWriter sr = File.AppendText("./logs/master.txt"))
            {
                sr.WriteLine("<====================== New Instance ======================>");
                sr.WriteLine($"Datetime: {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow:HH:mm:ss}");
                sr.WriteLine($"Mode: {mode}");
            }
        }

        public static Guid CreateRun()
        {
            Guid guidForRun = Uuid();

            Directory.CreateDirectory($"./run/{guidForRun.ToString("N").ToUpper()}");

            WriteLineToRunFile(guidForRun, "<====================== Begin New Run ======================>");
            WriteLineToRunFile(guidForRun, $"Datetime: {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow:HH:mm:ss}");
            WriteLineToRunFile(guidForRun, $"Run Object Guid: {guidForRun.ToString().ToUpper()}");

            WriteLineToMaster($"New Run Started with GUID {guidForRun.ToString().ToUpper()}");

            return guidForRun;
        }

        public static void WriteLineToRunFile(Guid currentGuid, string message)
        {
            using (StreamWriter sr = File.AppendText($"./logs/{currentGuid}.txt"))
                sr.WriteLine($"{message}");
        }

        public static void WriteLineToMaster(string message)
        {
            using (StreamWriter sr = File.AppendText("./logs/master.txt"))
                sr.WriteLine($"{DateTime.UtcNow:HH:mm:ss} || {message}");
        }

        public static void SaveBitmap(Guid currentGuid, double[,] image, string name)
        {
            Bitmap toSaveBitmap = image.ToBitmap();
            if (!Directory.Exists($"./run/{currentGuid.ToString("N").ToUpper()}"))
                throw new LoggerException("Run Directory Not Found, Logger Not Initialized Correctly");

            toSaveBitmap.Save($"./run/{currentGuid.ToString("N").ToUpper()}/{name}.png");
        }

        public static Guid Uuid() => Guid.NewGuid();
    }
}
