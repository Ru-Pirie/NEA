using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FinalSolution.src.utility
{
    static class Menu
    {
        public static object ScreenLock { get; } = new object();

        public static int CurrentLine { get; private set; } = 1;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int handle);

        // Check if window is as big as it can be
        public static bool IsWindowMax() => Console.WindowHeight >= Console.LargestWindowHeight && Console.WindowWidth >= Console.LargestWindowWidth - 3;

        private static double _progressBar;
        private static double _progressInterval;
        private static string _progressOutline;
        private static string _progressLine;

        // Draw the outline of the info box at the bottom of the console
        private static void LoadInfoBox()
        {
            for (int i = 0; i < Console.WindowWidth * 3 / 4; i++)
            {
                Console.SetCursorPosition(i, Console.WindowHeight * 5 / 6);
                Console.Write('-');
            }

            Console.SetCursorPosition(1, Console.WindowHeight * 5 / 6 + 2);
            Console.WriteLine("Current Page: Main Menu");
            Console.SetCursorPosition(1, Console.WindowHeight * 5 / 6 + 3);
            Console.WriteLine("Runtime: ");


            Console.SetCursorPosition(1, Console.WindowHeight * 5 / 6 + 8);
            Console.WriteLine("Author: Rubens Pirie");
            Console.SetCursorPosition(1, Console.WindowHeight * 5 / 6 + 9);
            Console.WriteLine("Name: Algorithmic Map Recognition and Edge Detection with Point to Point Pathfinding");
        }

        // Clear and redraw the outline of the logging box along with key
        public static void LoadLogBox()
        {

            for (int i = 0; i < Console.WindowHeight; i++)
            {
                if (i > 5)
                {
                    for (int j = Console.WindowWidth * 3 / 4; j < Console.WindowWidth; j++)
                    {
                        Console.SetCursorPosition(j, i);
                        Console.Write(' ');
                    }
                }

                Console.SetCursorPosition(Console.WindowWidth * 3 / 4, i);
                Console.Write('|');
            }

            for (int i = Console.WindowWidth * 3 / 4 + 1; i < Console.WindowWidth; i++)
            {
                Console.SetCursorPosition(i, 5);
                Console.Write('-');
            }

            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 1);
            Console.WriteLine("Program Logs:");
            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 3);
            Console.WriteLine("\x1b[48;5;9m  \x1b[0m ERROR            \x1b[48;5;10m  \x1b[0m EVENT PROCESSED");
            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 4);
            Console.WriteLine("\x1b[48;5;3m  \x1b[0m WARNING          \x1b[48;5;5m  \x1b[0m END OF SEQUENCE");
        }

        // Called once at the beginning of the program to set colour and render parts of the screen
        public static void RenderScreen()
        {
            IntPtr handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);

            Console.Clear();

            lock (ScreenLock)
            {
                LoadLogBox();
                LoadInfoBox();
            }
        }

        // Subroutine for updating the dynamic components of the info box
        public static void InfoBoxLoop(System.Diagnostics.Stopwatch sw)
        {
            while (true)
            {
                lock (ScreenLock)
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(15, Console.WindowHeight * 5 / 6 + 3);
                    Console.Write($"{sw.Elapsed.Hours}:{sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}".PadRight(10));
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public static void ClearUserSection()
        {
            CurrentLine = 1;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Console.WindowWidth * 3 / 4; i++) sb.Append(' ');

            string line = sb.ToString();

            lock (ScreenLock)
            {
                for (int i = 0; i < Console.WindowHeight * 5 / 6; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write(line);
                }
            }

            Console.SetCursorPosition(0, 0);
        }

        public static void SetPage(string message)
        {
            lock (ScreenLock)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(15, Console.WindowHeight * 5 / 6 + 2);
                Console.Write(message.PadRight(Console.WindowWidth * 3 / 4 - 15));
            }
        }

        public static void WriteLine()
        {
            if (CurrentLine > Console.WindowHeight * 5 / 6) ClearUserSection();
            CurrentLine++;
        }

        public static void WriteLine(string message)
        {
            Console.CursorVisible = false;

            if (message.Length > Console.WindowWidth * 3 / 4)
            {
                int maxLength = Console.WindowWidth * 3 / 4;

                List<string> words = message.Split(' ').ToList();
                StringBuilder sb = new StringBuilder();

                foreach (string word in words)
                {
                    if ($"{sb} {word}".Length > maxLength)
                    {
                        WriteLine(sb.ToString());
                        sb.Remove(0, sb.Length);
                    }
                    else
                    {
                        sb.Append($"{word} ");
                    }
                }

                WriteLine(sb.ToString());
            }
            else
            {
                lock (ScreenLock)
                {
                    if (CurrentLine > Console.WindowHeight * 5 / 6) ClearUserSection();

                    Console.SetCursorPosition(1, CurrentLine++);
                    Console.Write(message);
                }
            }
        }

        private static void ClearProgressBar()
        {
            _progressBar = 0;
            _progressInterval = 0;
            _progressLine = "";
            ClearUserSection();
        }

        public static void UpdateProgressBar()
        {
            lock (ScreenLock)
            {
                _progressBar = _progressBar + _progressInterval > 1 ? 1 : _progressBar + _progressInterval;

                int middle = Console.WindowHeight * 5 / 12;
                double possibleLength = (Console.WindowWidth * 3 / 4) - 4;
                possibleLength *= _progressBar;

                if (_progressLine.Length != (int)possibleLength)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < possibleLength; i++) sb.Append('|');
                    _progressLine = sb.ToString();

                    Console.SetCursorPosition(2, middle);
                    Console.Write($"\x1b[38;5;4m{_progressLine}\x1b[0m");
                }
            }
        }

        public static void SetupProgressBar(string title, int segments)
        {
            ClearProgressBar();
            _progressInterval = (double)1 / segments;

            StringBuilder bar = new StringBuilder();
            bar.Append('+');
            for (int i = 0; i < (Console.WindowWidth * 3 / 4) - 4; i++) bar.Append('-');
            bar.Append('+');

            _progressOutline = bar.ToString();
            DisplayProgressBar(title);
        }

        private static void DisplayProgressBar(string title)
        {
            lock (ScreenLock)
            {
                int middle = Console.WindowHeight * 5 / 12;

                lock (ScreenLock)
                {
                    Console.SetCursorPosition((Console.WindowWidth * 3 / 8) - (title.Length / 2), middle - 3);
                    Console.Write(title);

                    Console.SetCursorPosition(1, middle - 1);
                    Console.Write(_progressOutline);
                    Console.SetCursorPosition(1, middle);
                    Console.Write('|');
                    Console.SetCursorPosition(Console.WindowWidth * 3 / 4 - 2, middle);
                    Console.Write('|');
                    Console.SetCursorPosition(1, middle + 1);
                    Console.Write(_progressOutline);
                }
            }
        }
    }
}