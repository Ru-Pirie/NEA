using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalSolution.src.utility
{
    static class Menu
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
            if (clear) Console.Clear();

            Console.WriteLine(title);

            foreach (var option in options) Console.WriteLine($"  {option}");

            bool selected = false;

            Console.CursorVisible = false;

            Console.SetCursorPosition(0, 1);
            Console.Write('>');

            while (!selected)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.DownArrow && Console.CursorTop < options.Count())
                {
                    Console.CursorLeft = 0;
                    Console.Write(' ');
                    Console.CursorTop++;
                    Console.CursorLeft = 0;
                    Console.Write('>');
                }
                else if (key.Key == ConsoleKey.UpArrow && Console.CursorTop > 1)
                {
                    Console.CursorLeft = 0;
                    Console.Write(' ');
                    Console.CursorTop--;
                    Console.CursorLeft = 0;
                    Console.Write('>');
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (clear)
                    {
                        Console.Clear();
                        Console.SetCursorPosition(0, 0);
                    }
                    
                    Console.CursorVisible = true;
                    return Console.CursorTop;
                }
            }

            throw new MenuException("An unexpected error occured", new Exception("Forbiden line reached"));
        }


    }
}
