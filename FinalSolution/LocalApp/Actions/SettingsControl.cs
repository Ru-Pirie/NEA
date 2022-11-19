using LocalApp.CLI;
using System;
using System.Collections.Generic;

namespace LocalApp
{
    public class SettingsControl
    {
        private readonly Settings _settings;
        private readonly Menu _menuInstance;
        private readonly Log _logInstance;
        private readonly Input _inputHandel;

        private readonly Dictionary<string, (string, Type)> _oldSettings;

        public SettingsControl(Settings settings, Menu menuInstance, Log logInstance)
        {
            _settings = settings;
            _menuInstance = menuInstance;
            _logInstance = logInstance;
            _oldSettings = new Dictionary<string, (string, Type)>(Settings.UserSettings);
            _inputHandel = new Input(_menuInstance);
        }

        public void Start()
        {
            bool running = true;

            while (running)
            {
                _menuInstance.SetPage("Settings Home Page");
                int opt = _inputHandel.GetOption("Whcih settings would you like to change?",
                    new[]
                    {
                        "General",
                        "Pathfinding",
                        "Save",
                        "Algorithm",
                        "Exit"
                    }
                );

                switch (opt)
                {
                    case 0:
                        _menuInstance.SetPage("Settings -> General Settings");
                        General();
                        break;
                    case 1:
                        _menuInstance.SetPage("Settings -> Pathfinding Settings");
                        Pathfinding();
                        break;
                    case 2:
                        _menuInstance.SetPage("Settings -> Save Settings");
                        Save();
                        break;
                    case 3:
                        _menuInstance.SetPage("Settings -> Pathfinding Algorithm");

                        int algorithmOption = _inputHandel.GetOption("Select which pathfinding algorithm you wish to use:", new string[] {
                            "Dijkstra",
                            "AStar"
                        });

                        string newValue = algorithmOption == 0 ? "Dijkstra" : "AStar";

                        _settings.Change("pathfindingAlgorithm", newValue);
                        break;
                    default:
                        running = false;

                        _settings.Update(_oldSettings, Settings.UserSettings);

                        break;

                }
            }
        }

        private void General()
        {
            (string, bool)[] settings = new (string, bool)[] {
                ( "detailedLogging", bool.Parse(Settings.UserSettings["detailedLogging"].Item1)),
                ( "forceFormsFront", bool.Parse(Settings.UserSettings["forceFormsFront"].Item1)),
            };

            IEnumerable<(string, bool)> result = _inputHandel.OptionSelector("General Settings:", settings);
            foreach (var item in result) _settings.Change(item.Item1, item.Item2);
        }

        private void Pathfinding()
        {
            (string, bool)[] settings = new (string, bool)[] {
                ( "convertToMST", bool.Parse(Settings.UserSettings["convertToMST"].Item1)),
                ( "snapToGrid", bool.Parse(Settings.UserSettings["snapToGrid"].Item1)),
                ( "endOnFind", bool.Parse(Settings.UserSettings["endOnFind"].Item1)),
            };

            IEnumerable<(string, bool)> result = _inputHandel.OptionSelector("Save File Settings:", settings);
            foreach (var item in result) _settings.Change(item.Item1, item.Item2);
        }

        private void Save()
        {
            (string, bool)[] settings = new (string, bool)[] {
                ( "shortNames", bool.Parse(Settings.UserSettings["shortNames"].Item1)),
                ( "zipOnComplete", bool.Parse(Settings.UserSettings["zipOnComplete"].Item1)),
            };

            IEnumerable<(string, bool)> result = _inputHandel.OptionSelector("Save File Settings:", settings);
            foreach (var item in result) _settings.Change(item.Item1, item.Item2);
        }

    }
}
