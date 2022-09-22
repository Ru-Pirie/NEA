using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalApp.CLI
{
    internal class Menu
    {
        public object ScreenLock { get; } = new object();
        public int CurrentLine { get; private set; } = 1;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int handle);

        public bool IsWindowMax() => Console.WindowHeight >= Console.LargestWindowHeight && Console.WindowWidth >= Console.LargestWindowWidth - 3;

        private readonly string _permLineA;
        private readonly string _permLineB;

        private const char _verticalChar = '│';
        private const char _horizontalChar = '─';

        public Menu(string permLineA, string permLineB)
        {
            IntPtr handle = GetStdHandle(-11);
            GetConsoleMode(handle, out var mode);
            SetConsoleMode(handle, mode | 0x4);

            _permLineA = permLineA;
            _permLineB = permLineB;

            Console.Clear();
        }

        public void Setup()
        {
            while (!IsWindowMax())
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("\x1b[38;5;9mMaximize Window To Continue\x1b[0m");
                System.Threading.Thread.Sleep(250);
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("\x1b[48;5;9mMaximize Window To Continue\x1b[0m");
                System.Threading.Thread.Sleep(250);
                
            }

            Console.Clear();

            DisplayInfoBox();
            DisplayLogBox();

            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            new Task(() => BeginInfoLoop(Stopwatch.StartNew())).Start();
        }

        private void DisplayInfoBox()
        {
            for (int i = 0; i < Console.WindowWidth * 3 / 4; i++)
            {
                Console.SetCursorPosition(i, Console.WindowHeight * 5 / 6);
                Console.Write(_horizontalChar);
            }

            Console.SetCursorPosition(1, Console.WindowHeight * 5 / 6 + 2);
            Console.WriteLine("Current Page: ????? ??? ??????");
            Console.SetCursorPosition(1, Console.WindowHeight * 5 / 6 + 3);
            Console.WriteLine("Runtime:       ??:??:??");

            Console.SetCursorPosition(1, Console.WindowHeight * 5 / 6 + 8);
            Console.WriteLine(_permLineA);
            Console.SetCursorPosition(1, Console.WindowHeight * 5 / 6 + 9);
            Console.WriteLine(_permLineB);
        }

        private void DisplayLogBox()
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
                Console.Write(_verticalChar);
            }

            for (int i = Console.WindowWidth * 3 / 4 + 1; i < Console.WindowWidth; i++)
            {
                Console.SetCursorPosition(i, 5);
                Console.Write(_horizontalChar);
            }

            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 1);
            Console.WriteLine("Program Logs:");
            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 3);
            Console.WriteLine("\x1b[48;5;9m  \x1b[0m ERROR            \x1b[48;5;10m  \x1b[0m EVENT PROCESSED");
            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 4);
            Console.WriteLine("\x1b[48;5;3m  \x1b[0m WARNING          \x1b[48;5;5m  \x1b[0m END OF SEQUENCE");
        }

        private void BeginInfoLoop(Stopwatch sw)
        {
            while (true)
            {
                lock (ScreenLock)
                {
                    Console.SetCursorPosition(15, Console.WindowHeight * 5 / 6 + 3);
                    Console.Write($"{sw.Elapsed.Hours}:{sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}".PadRight(10, ' '));
                    Console.CursorVisible = false;
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void ClearLogSection()
        {
            for (int i = 6; i < Console.WindowHeight; i++)
            {
                for (int j = Console.WindowWidth * 3 / 4 + 1; j < Console.WindowWidth; j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.Write(' ');
                }
            }
        }

        public void ClearUserSection()
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

        public void SetPage(string message)
        {
            lock (ScreenLock)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(15, Console.WindowHeight * 5 / 6 + 2);
                Console.Write(message.PadRight(Console.WindowWidth * 3 / 4 - 15));
            }

            Console.Title = $"Comp Sci NEA | Rubens Pirie | {message}";
        }

        public void WriteLine()
        {
            if (CurrentLine > Console.WindowHeight * 5 / 6) ClearUserSection();
            CurrentLine++;
        }

        public void WriteLine(string message)
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

    }
}
