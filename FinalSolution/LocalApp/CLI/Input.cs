using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalApp.CLI
{
    public class Input
    {
        private readonly Menu _menuInstance;

        public Input(Menu menuInstance)
        {
            _menuInstance = menuInstance;
        }

        /// <summary>
        /// A function to easily display a menu and get an option from a supplied list.
        /// </summary>
        /// <param name="title">Title of the menu to be displayed</param>
        /// <param name="options">Options to be displayed</param>
        /// <param name="clear">Clear the screen on function call</param>
        /// <returns>0 based index for the option which was selected</returns>
        public int GetOption(string title, IEnumerable<string> options, bool clear = true)
        {
            while (Console.KeyAvailable) Console.ReadKey(true);
            _menuInstance.ClearUserSection();
            _menuInstance.WriteLine(title);

            int j = 3;

            lock (_menuInstance.ScreenLock)
            {
                foreach (var option in options)
                {
                    Console.SetCursorPosition(1, j++);
                    Console.WriteLine($"  {option}");
                }
            }

            bool selected = false;
            int currentTop;

            lock (_menuInstance.ScreenLock)
            {
                Console.SetCursorPosition(1, 3);
                Console.Write('>');

                currentTop = Console.CursorTop;
            }

            while (!selected)
            {
                Console.CursorVisible = false;

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.DownArrow && currentTop < options.Count() + 2)
                {
                    lock (_menuInstance.ScreenLock)
                    {
                        Console.CursorLeft = 1;
                        Console.CursorTop = currentTop;
                        Console.Write(' ');
                        Console.CursorTop = ++currentTop;
                        Console.CursorLeft = 1;
                        Console.Write('>');
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow && currentTop > 3)
                {
                    lock (_menuInstance.ScreenLock)
                    {
                        Console.CursorLeft = 1;
                        Console.CursorTop = currentTop;
                        Console.Write(' ');
                        Console.CursorTop = --currentTop;
                        Console.CursorLeft = 1;
                        Console.Write('>');
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (clear) _menuInstance.ClearUserSection();
                    Console.CursorVisible = false;

                    selected = true;
                }
            }

            return currentTop - 3;
        }

        public void WaitInput(string prompt)
        {
            while (Console.KeyAvailable) Console.ReadKey(true);
            _menuInstance.WriteLine(prompt);
            bool complete = false;

            while (!complete)
            {
                if (!Console.KeyAvailable) continue;
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) complete = true;
            }
        }

        public IEnumerable<(string, bool)> OptionSelector(string title, IEnumerable<(string, bool)> options, bool clear = true)
        {
            List<(string, bool)> result = new List<(string, bool)>(options);
            result.Add(("EXIT", false));

            while (Console.KeyAvailable) Console.ReadKey(true);
            _menuInstance.ClearUserSection();
            _menuInstance.WriteLine(title);

            int j = 3;

            lock (_menuInstance.ScreenLock)
            {
                foreach (var option in result)
                {
                    Console.SetCursorPosition(1, j++);
                    if (option.Item2) Console.WriteLine($"  {option.Item1} [{Log.Green}x{Log.Blank}]");
                    else Console.WriteLine($"  {option.Item1} [ ]");
                }
            }

            bool selected = false;
            int currentTop;

            lock (_menuInstance.ScreenLock)
            {
                Console.SetCursorPosition(1, 3);
                Console.Write('>');

                currentTop = Console.CursorTop;
            }

            while (!selected)
            {
                Console.CursorVisible = false;

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.DownArrow && currentTop < result.Count() + 2)
                {
                    lock (_menuInstance.ScreenLock)
                    {
                        Console.CursorLeft = 1;
                        Console.CursorTop = currentTop;
                        Console.Write(' ');
                        Console.CursorTop = ++currentTop;
                        Console.CursorLeft = 1;
                        Console.Write('>');
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow && currentTop > 3)
                {
                    lock (_menuInstance.ScreenLock)
                    {
                        Console.CursorLeft = 1;
                        Console.CursorTop = currentTop;
                        Console.Write(' ');
                        Console.CursorTop = --currentTop;
                        Console.CursorLeft = 1;
                        Console.Write('>');
                    }
                }
                else if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
                {
                    if (result.Count + 2 == currentTop)
                    {
                        if (clear) _menuInstance.ClearUserSection();
                        Console.CursorVisible = false;

                        selected = true;
                    }
                    else
                    {
                        result[currentTop - 3] = (result[currentTop - 3].Item1, !result[currentTop - 3].Item2);
                        Console.SetCursorPosition(1, currentTop);
                        if (result[currentTop - 3].Item2) Console.WriteLine($"> {result[currentTop - 3].Item1} [{Log.Green}x{Log.Blank}]");
                        else Console.WriteLine($"> {result[currentTop - 3].Item1} [ ]");
                    }
                }
            }

            return result;
        }


        public string GetInput(string prompt)
        {
            while (Console.KeyAvailable) Console.ReadKey(true);
            _menuInstance.WriteLine(prompt);

            bool complete = false;
            StringBuilder input = new StringBuilder();
            int line = _menuInstance.CurrentLine;

            while (!complete)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            complete = true;
                            break;
                        case ConsoleKey.Backspace:
                        case ConsoleKey.Delete:
                            {
                                if (input.Length > 0)
                                {
                                    lock (_menuInstance.ScreenLock)
                                    {
                                        Console.SetCursorPosition((input.Length % (Console.WindowWidth * 3 / 4 - 1)), line);
                                        Console.Write(' ');
                                    }

                                    input.Remove(input.Length - 1, 1);
                                }

                                break;
                            }
                        default:
                            {
                                if (input.Length / (line - 1) > Console.WindowWidth * 3 / 4 - 2) line++;

                                lock (_menuInstance.ScreenLock)
                                {
                                    Console.SetCursorPosition((input.Length % (Console.WindowWidth * 3 / 4 - 1)) + 1, line);
                                    Console.Write(key.KeyChar);
                                }

                                input.Append(key.KeyChar);
                                break;
                            }
                    }
                }
            }

            _menuInstance.WriteLine();

            return input.ToString();
        }

        public double GetDouble(string prompt) => double.Parse(GetInput(prompt));

        public bool TryGetDouble(string prompt, out double result) => double.TryParse(GetInput(prompt), out result);

        public int GetInt(string prompt) => int.Parse(GetInput(prompt));

        public bool TryGetInt(string prompt, out int result) => int.TryParse(GetInput(prompt), out result);



    }
}
