using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using FinalSolution.src.local ;
using FinalSolution.src.utility;

namespace FinalSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Rubens Pirie Compsci NEA | Boot";
            int opt = Prompt.GetOption("Please select the version you would like to use:", new [] { "Local version", "Web version", "Do Special Stuff" });
            switch (opt)
            {
                case 0:
                    Console.Title = "Rubens Pirie Compsci NEA | Local";
                    Console.WriteLine("Before you begin please full screen the console.\nPress enter to continue...");
                    while (!Menu.IsWindowMax())
                    {
                        Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Maximise window! " +
                            $"(Current: {Console.WindowWidth} x {Console.WindowHeight} | Max: {Console.LargestWindowWidth} x {Console.LargestWindowWidth})");
                        Console.ResetColor();
                    }
                    StartLocal();
                    break;
                case 1:
                    Console.Title = "Rubens Pirie Compsci NEA | Cloud";
                    StartBlazor();
                    break;
                case 2:
                    CannyEdgeDetection thing = new CannyEdgeDetection(new Bitmap("./image.jpg"));
                    break;
            }
        }

        private static void StartLocal()
        {
            Menu.RenderScreen();
            new LocalApplication();
        }

        private static void StartBlazor()
        {
            throw new NotImplementedException("This is an extenstion objective and has not been completed yet.");
        }
    }
}