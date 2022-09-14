using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BackendLib;

namespace LocalApp.CLI
{
    internal class Log
    {
        private int _logLineCount = 6;
        private Menu _menuInstance;

        public void Error(string message) => Logger.WriteLineToMaster($"ERROR {message}");
        public void Warn(string message) => Logger.WriteLineToMaster($"WARNING {message}");
        public void Event(string message) => Logger.WriteLineToMaster($"EVENT {message}");
        public void End(string message) => Logger.WriteLineToMaster($"END {message}");

        public void Error(Guid runGuid, string message) => LogParent(runGuid, message, 0);
        public void Warn(Guid runGuid, string message) => LogParent(runGuid, message, 1);
        public void Event(Guid runGuid, string message) => LogParent(runGuid, message, 2);
        public void End(Guid runGuid, string message) => LogParent(runGuid, message, 3);

        public void EndErrorRun(Guid runGuid, Exception ex)
        {
            Error($"Run ({runGuid}) terminated due to an error.");
            Error($"Exception: {ex.Message}");
            if (ex.InnerException != null) Error($"Inner Exception: {ex.InnerException.Message}");
            Error(runGuid, ex.Message);
            End(runGuid, $"Run ({runGuid}) terminated.");
        }

        public void EndSuccessRun(Guid runGuid)
        {
            End(runGuid, "Successfully completed processing and pathfinding of new image!");
            Warn(runGuid, $"Run Guid {runGuid} Deleted. See {Environment.CurrentDirectory}\\saves\\ for output(s) and {Environment.CurrentDirectory}\\runs\\{runGuid.ToString("N").ToUpper()} for temp images.");
            End($"Completed run {runGuid} successfully.");
        }

        public Log(Menu menuInstance)
        {
            _menuInstance = menuInstance;
            _ = new Logger(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type">0 - Error, 1 - Warning, 2 - Event, 3 - End</param>
        private void LogParent(Guid runGuid, string message, int type)
        {
            Console.CursorVisible = false;
            string[] prefix = { "\x1b[38;5;9mERROR\x1b[0m", "\x1b[38;5;3m WARN\x1b[0m", "\x1b[38;5;10mEVENT\x1b[0m", "\x1b[38;5;5m  END\x1b[0m" };
            string[] filePrefix = { "[ERROR] ", "[WARN] ", "[EVENT] ", "[END] " };

            lock (_menuInstance.ScreenLock)
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

            Logger.WriteLineToRunFile(runGuid, $"{filePrefix[type]}{message}");
        }

        // Make sure that the total log lines does not exceed the space given
        private bool CheckLogLineCount()
        {
            if (_logLineCount >= Console.WindowHeight)
            {
                _logLineCount = 6;
                _menuInstance.ClearLogSection();

                return true;
            }

            return false;
        }
    }
}
