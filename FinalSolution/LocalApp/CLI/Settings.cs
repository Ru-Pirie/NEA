using BackendLib.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace LocalApp.CLI
{
    public class Settings
    {
        private readonly Menu _menuInstance;
        private readonly Log _loggerInstance;

        private List<string> rawLines;
        public static Dictionary<string, (string, Type)> UserSettings { get; private set; }

        private readonly string[] defaultSettings = {
            "# Manually Edit At Own Risk",
            "# General Settings",
            "detailedLogging=false",
            "forceFormsFront=true",
            "",
            "# Pathfinding Settings",
            "convertToMST=false",
            "pathfindingAlgorithm=AStar",
            "snapToGrid=true",
            "endOnFind=false",
            "",
            "# Save Settings",
            "shortNames=false",
            "zipOnComplete=false",
        };

        public Settings(Menu menu, Log log)
        {
            _menuInstance = menu;
            _loggerInstance = log;
        }

        public void CheckIfExistsOrCreate()
        {
            if (!File.Exists("settings.conf"))
            {
                _loggerInstance.Event("Settings file did not exist. Creating...");
                using (TextWriter tw = File.CreateText("settings.conf"))
                {
                    foreach (string line in defaultSettings)
                    {
                        tw.WriteLine(line);
                    }
                }
            }
        }

        public List<string> ParseSettingsFile()
        {
            List<string> lines = new List<string>();
            using (StreamReader sr = File.OpenText("settings.conf"))
            {
                while (!sr.EndOfStream)
                {
                    lines.Add(sr.ReadLine());
                }
            }

            rawLines = lines;

            List<string> validLines = new List<string>();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Trim() != "" && !lines[i].Trim().StartsWith("#")) validLines.Add(lines[i]);
            }

            return validLines;
        }

        private Dictionary<string, (string, Type)> ConvertSettingsToPairs(List<string> parsedLines)
        {
            Dictionary<string, (string, Type)> pairs = new Dictionary<string, (string, Type)>();
            foreach (string item in parsedLines)
            {
                string name = item.Split('=')[0].Trim();
                string value = item.Split('=')[1].Trim();
                if (bool.TryParse(value, out bool _)) pairs.Add(name, (value, typeof(bool)));
                else if (int.TryParse(value, out int _)) pairs.Add(name, (value, typeof(int)));
                else if (double.TryParse(value, out double _)) pairs.Add(name, (value, typeof(double)));
                else pairs.Add(name, (value, typeof(string)));
            }

            return pairs;
        }

        public bool Change(string setting, bool value)
        {
            if (!UserSettings.ContainsKey(setting)) return false;
            UserSettings[setting] = (value.ToString().ToLower(), typeof(bool));

            return true;
        }

        public bool Change(string setting, int value)
        {
            if (!UserSettings.ContainsKey(setting)) return false;
            UserSettings[setting] = (value.ToString(), typeof(int));

            return true;
        }

        public bool Change(string setting, double value)
        {
            if (!UserSettings.ContainsKey(setting)) return false;
            UserSettings[setting] = (value.ToString(), typeof(double));

            return true;
        }

        public bool Change(string setting, string value)
        {
            if (!UserSettings.ContainsKey(setting)) return false;
            UserSettings[setting] = (value.ToString(), typeof(string));

            return true;
        }

        public void Read()
        {
            CheckIfExistsOrCreate();
            List<string> parsedLines = ParseSettingsFile();
            Dictionary<string, (string, Type)> settingValuePairs = ConvertSettingsToPairs(parsedLines);
            UserSettings = settingValuePairs;
        }

        public void Update(Dictionary<string, (string, Type)> oldSettings, Dictionary<string, (string, Type)> newSettings)
        {
            if (oldSettings.Count != newSettings.Count) throw new SettingsException("Cannot set settings when the amount of settings has changed, if this problem persists delete settings.conf and restart the program.");

            foreach (KeyValuePair<string, (string, Type)> pair in newSettings)
            {
                int location = rawLines.FindIndex(toCheck => toCheck.Contains(pair.Key));
                if (location == -1) throw new SettingsException($"You have an unknown setting {pair.Key}, if this problem persists delete settings.conf and restart the program.");
                else
                {
                    if (!oldSettings.ContainsKey(pair.Key)) throw new SettingsException($"Setting {pair.Key} does not exist, if this problem persists delete settings.conf and restart the program.");
                    if (!oldSettings[pair.Key].Equals(pair.Value)) rawLines[location] = $"{pair.Key}= {pair.Value.Item1}";
                }
            }

            Write();
        }

        private void Write()
        {
            using (TextWriter tw = File.CreateText("settings.conf"))
            {
                foreach (string line in rawLines)
                {
                    tw.WriteLine(line);
                }
            }
        }
    }
}
