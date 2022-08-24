using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSolution.src.utility
{
    static class Log
    {
        private static int _logLineCount = 6;

        public static void Error(string message) => MasterLog(message, 0);
        public static void Warn(string message) => MasterLog(message, 1);
        public static void Event(string message) => MasterLog(message, 2);
        public static void End(string message) => MasterLog(message, 3);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type">0 - Error, 1 - Warning, 2 - Event, 3 - End</param>
        private static void MasterLog(string message, int type)
        {
            Console.CursorVisible = false;
            string[] prefix = { "\x1b[38;5;9mERROR\x1b[0m", "\x1b[38;5;3m WARN\x1b[0m", "\x1b[38;5;10mEVENT\x1b[0m", "\x1b[38;5;5m  END\x1b[0m" };

            lock (Menu.ScreenLock)
            {
                CheckLogLineCount();

                if (message.Length > Console.WindowWidth / 4 - 7)
                {
                    Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 2, _logLineCount++);
                    int i = 10;

                    Console.Write($"{prefix[type]}: ");
                    
                    foreach (char letter in message)
                    {
                        Console.Write(letter);
                        i++;
                        if (i > Console.WindowWidth / 4)
                        {
                            if (CheckLogLineCount()) return;
                            Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 9, _logLineCount++);
                            i = 10;
                        }
                    }
                }
                else
                {
                    Console.SetCursorPosition(Console.WindowWidth * 3 / 4 + 2, _logLineCount++);
                    Console.Write($"{prefix[type]}: {message}");
                }
            }
        }

        // Make sure that the total log lines does not exceed the space given
        private static bool CheckLogLineCount()
        {
            if (_logLineCount >= Console.WindowHeight)
            {
                _logLineCount = 6;
                Menu.LoadLogBox();

                return true;
            }

            return false;
        }
    }
}
