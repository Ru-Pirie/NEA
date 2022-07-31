using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalSolution.src.utility
{
    static class Prompt
    {
        /// <summary>
        /// A function to easily display a menu and get an option from a supplied list.
        /// </summary>
        /// <param name="title">Title of the menu to be displayed</param>
        /// <param name="options">Options to be displayed</param>
        /// <param name="clear">Clear the screen on function call</param>
        /// <exception cref="MenuException"></exception>
        /// <returns>0 based index for the option which was selected</returns>
        public static int GetOption(string title, IEnumerable<string> options, bool clear = true)
        {
            Menu.ClearUserSection();

            lock (Menu.ScreenLock)
            {
                Console.SetCursorPosition(1, 1);
                Console.Write(title);
            }

            int j = 3;

            lock (Menu.ScreenLock)
            {
                foreach (var option in options)
                {
                    Console.SetCursorPosition(1, j++);
                    Console.WriteLine($"  {option}");
                }
            }

            bool selected = false;
            int currentTop;

            lock (Menu.ScreenLock)
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
                    lock (Menu.ScreenLock)
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
                    lock (Menu.ScreenLock)
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
                    if (clear) Menu.ClearUserSection();
                    Console.CursorVisible = false;

                    return currentTop - 3;
                }
            }

            throw new MenuException("An unexpected error occurred", new Exception("Forbidden line reached"));
        }

        public static string GetInput(string prompt)
        {
            // Clear buffer
            while (Console.KeyAvailable) Console.ReadKey(true);
            Menu.WriteLine(prompt);

            bool complete = false;
            StringBuilder input = new StringBuilder();
            int line = Menu.CurrentLine;

            while (!complete)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)    
                    {
                        complete = true;
                    }
                    else if (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Delete)
                    {
                        if (input.Length > 0)
                        {
                            lock (Menu.ScreenLock)
                            {
                                Console.SetCursorPosition((input.Length % (Console.WindowWidth * 3 / 4 - 1)), line);
                                Console.Write(' ');
                            }

                            input.Remove(input.Length - 1, 1);
                        }
                    }
                    else
                    {
                        if (input.Length / (line - 1) > Console.WindowWidth * 3 / 4 - 2) line++;

                        lock (Menu.ScreenLock)
                        {
                            Console.SetCursorPosition((input.Length % (Console.WindowWidth * 3 / 4 - 1)) + 1, line);
                            Console.Write(key.KeyChar);
                        }

                        input.Append(key.KeyChar);
                    }
                }
            }

            Menu.WriteLine();

            return input.ToString();
        }
    }
}
