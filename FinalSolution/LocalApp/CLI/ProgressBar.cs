using System;
using System.Text;

namespace LocalApp.CLI
{
    public class ProgressBar
    {
        private readonly string _progressTitle;
        private double _progressAmount;
        private readonly double _progressInterval;
        private readonly string _progressOutline;
        private string _progressLine;

        private readonly Menu _menuInstance;

        public ProgressBar(string title, int totalSegments, Menu menuInstance)
        {
            _progressInterval = (double)1 / totalSegments;
            _progressAmount = 0;

            StringBuilder bar = new StringBuilder();
            bar.Append('+');
            for (int i = 0; i < (Console.WindowWidth * 3 / 4) - 4; i++) bar.Append('-');
            bar.Append('+');

            _progressOutline = bar.ToString();
            _progressLine = "";
            _progressTitle = title;
            _menuInstance = menuInstance;
        }

        public void DisplayProgress()
        {
            int middle = Console.WindowHeight * 5 / 12;

            lock (_menuInstance.ScreenLock)
            {
                Console.SetCursorPosition((Console.WindowWidth * 3 / 8) - (_progressTitle.Length / 2), middle - 3);
                Console.Write(_progressTitle);

                Console.SetCursorPosition(1, middle - 1);
                Console.Write(_progressOutline);
                Console.SetCursorPosition(1, middle);
                Console.Write('|');
                Console.SetCursorPosition(Console.WindowWidth * 3 / 4 - 2, middle);
                Console.Write('|');
                Console.SetCursorPosition(1, middle + 1);
                Console.Write(_progressOutline);
            }
        }

        public Action GetIncrementAction() => new Action(IncrementProgress);

        private void IncrementProgress()
        {
            lock (_menuInstance.ScreenLock)
            {
                _progressAmount = _progressAmount + _progressInterval > 1 ? 1 : _progressAmount + _progressInterval;

                int middle = Console.WindowHeight * 5 / 12;
                double possibleLength = (Console.WindowWidth * 3 / 4) - 4;
                possibleLength *= _progressAmount;

                if (_progressLine.Length != (int)possibleLength)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < possibleLength; i++) sb.Append('|');
                    _progressLine = sb.ToString();

                    Console.SetCursorPosition(2, middle);
                    Console.Write($"{Log.Blue}{_progressLine}{Log.Blank}");
                }
            }
        }
    }
}
