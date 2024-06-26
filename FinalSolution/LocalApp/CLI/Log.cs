﻿using BackendLib;
using System;

namespace LocalApp.CLI
{
    public class Log
    {
        private int _logLineCount = 6;
        private readonly Menu _menuInstance;

        public const string Red = "\x1b[38;5;196m";
        public const string Orange = "\x1b[38;5;184m";
        public const string Purple = "\x1b[38;5;129m";
        public const string Green = "\x1b[38;5;2m";
        public const string Blue = "\x1b[38;5;27m";
        public const string Pink = "\x1b[38;5;200m";
        public const string Grey = "\x1b[38;5;243m";
        public const string Blank = "\x1b[0m";

        public void Error(string message) => Logger.WriteLineToMaster($"ERROR {message}");
        public void Warn(string message) => Logger.WriteLineToMaster($"WARNING {message}");
        public void Event(string message) => Logger.WriteLineToMaster($"EVENT {message}");
        public void End(string message) => Logger.WriteLineToMaster($"END {message}");

        public void Error(Guid runGuid, string message, bool detailed = false) => LogParent(runGuid, message, 0, detailed);
        public void Warn(Guid runGuid, string message, bool detailed = false) => LogParent(runGuid, message, 1, detailed);
        public void Event(Guid runGuid, string message, bool detailed = false) => LogParent(runGuid, message, 2, detailed);
        public void End(Guid runGuid, string message, bool detailed = false) => LogParent(runGuid, message, 3, detailed);

        public void EndError(Guid runGuid, Exception ex)
        {
            Error($"Run ({runGuid}) terminated due to an error.");
            Error($"Exception: {ex.Message}");
            if (ex.InnerException != null) Error($"Inner Exception: {ex.InnerException.Message}");
            Error(runGuid, ex.Message);
            End(runGuid, $"Run ({runGuid}) terminated.", true);
        }

        public void EndSuccessRun(Guid runGuid)
        {
            End(runGuid, "Successfully completed processing and pathfinding of new image!", true);
            Warn(runGuid, $"Run Guid {runGuid} Deleted. See {Environment.CurrentDirectory}\\saves\\ for output(s) and {Environment.CurrentDirectory}\\runs\\{runGuid.ToString("N").ToUpper()} for temp images.", true);
            End($"Completed run {runGuid} successfully.");
        }

        public void EndSuccessSave(Guid runGuid)
        {
            End(runGuid, "Successfully completed recall and pathfinding of save file!", true);
            Warn(runGuid, $"Run Guid {runGuid} Deleted. See {Environment.CurrentDirectory}\\saves\\ for output(s). Or just go to where the save file was located.", true);
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
        private void LogParent(Guid runGuid, string message, int type, bool detailed)
        {
            if (bool.Parse(Settings.UserSettings["detailedLogging"].Item1) && detailed) return;

            Console.CursorVisible = false;
            string[] prefix = { $"{Red}ERROR{Log.Blank}", $"{Orange}WARN{Log.Blank}", $"{Green}EVENT{Log.Blank}", $"{Purple}END{Log.Blank}" };
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
