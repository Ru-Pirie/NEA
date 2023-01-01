using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalApp.CLI
{
    public class Menu
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

        public const char VerticalChar = '│';
        public const char HorizontalChar = '─';

        public Menu(string permLineA, string permLineB)
        {
            IntPtr handle = GetStdHandle(-11);
            GetConsoleMode(handle, out var mode);
            SetConsoleMode(handle, mode | 0x4);

            int width = Console.WindowWidth / 2;
            int height = Console.WindowHeight / 4;
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);

            _permLineA = permLineA;
            _permLineB = permLineB;

            Console.Clear();
            Console.CursorVisible = false;
        }

        public void Setup()
        {
            while (!IsWindowMax())
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"{Log.Red}Maximize Window To Continue{Log.Blank}");
                System.Threading.Thread.Sleep(250);
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"\x1b[48;5;196mMaximize Window To Continue{Log.Blank}");
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
                Console.Write(HorizontalChar);
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
                Console.Write(VerticalChar);
            }

            for (int i = Console.WindowWidth * 3 / 4 + 1; i < Console.WindowWidth; i++)
            {
                Console.SetCursorPosition(i, 5);
                Console.Write(HorizontalChar);
            }

            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 1);
            Console.WriteLine("Program Logs:");
            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 3);
            Console.WriteLine($"\x1b[48;5;196m  {Log.Blank} ERROR            \x1b[48;5;2m  {Log.Blank} EVENT PROCESSED");
            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 5, 4);
            Console.WriteLine($"\x1b[48;5;184m  {Log.Blank} WARNING          \x1b[48;5;129m  {Log.Blank} END OF SEQUENCE");
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
                for (int i = 0; i < ((int)(Console.WindowHeight * 5 / 6)) - 1; i++)
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

        public void Error(string message)
        {
            int widthStart = ((Console.WindowWidth * 3 / 4) / 3) / 2;
            int heightStart = (Console.WindowHeight * 5 / 6) / 3;
            for (int i = 0; i < widthStart * 4; i++)
            {
                lock (ScreenLock)
                {
                    string toPrint = i == 0 || i == widthStart * 4 - 1 ? "+" : HorizontalChar.ToString();
                    Console.SetCursorPosition(widthStart + i, heightStart);
                    Console.Write($"{toPrint}");
                    Console.SetCursorPosition(widthStart + i, heightStart * 2);
                    Console.Write($"{toPrint}");
                }
            }

            for (int i = heightStart + 1; i < heightStart * 2; i++)
            {
                lock (ScreenLock)
                {
                    Console.SetCursorPosition(widthStart, i);
                    Console.Write($"{VerticalChar}");
                    Console.SetCursorPosition(widthStart + widthStart * 4 - 1, i);
                    Console.Write($"{VerticalChar}");
                }
            }

            List<List<char>> messages = new List<List<char>>();
            messages.Add(new List<char>());
            List<char> messageChars = message.ToCharArray().ToList();
            messageChars.Reverse();

            int e = 0;
            while (messageChars.Count > 0)
            {
                if (messages[e].Count < widthStart * 3)
                {
                    messages[e].Add(messageChars[messageChars.Count - 1]);
                    messageChars.RemoveAt(messageChars.Count - 1);
                }
                else
                {
                    e++;
                    messages.Add(new List<char>());
                };
            }

            lock (ScreenLock)
            {
                Console.SetCursorPosition((widthStart * 3) - 26, heightStart + 2);
                Console.Write($"{Log.Red}Something went wrong, to see what take a look below.{Log.Blank}");
                Console.SetCursorPosition((widthStart * 3) - 8, (int)(heightStart * 1.5) - 3);
                Console.Write("Reason for Error");
                for (int i = 0; i < messages.Count; i++)
                {
                    Console.SetCursorPosition((widthStart * 3) - messages[i].Count / 2, (int)(heightStart * 1.5) - (2 - i));
                    Console.Write($"{Log.Blue}{string.Join("", messages[i])}{Log.Blank}");
                }
                Console.SetCursorPosition((widthStart * 3) - 18, heightStart * 2 - 2);
                Console.Write($"{Log.Grey}(Press Enter to Return to Main Menu){Log.Blank}");
            }


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
