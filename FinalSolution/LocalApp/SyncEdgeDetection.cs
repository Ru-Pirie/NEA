using BackendLib;
using BackendLib.Interfaces;
using LocalApp.CLI;
using System;

namespace LocalApp
{
    // Use inheritance?
    internal class SyncEdgeDetection : IHandler
    {
        private readonly Menu _menu;
        private readonly Input _input;
        private readonly Log _log;
        private readonly Structures.RawImage _image;
        private readonly Guid _runGuid;
        private double[,] _workingArray;

        public SyncEdgeDetection(Menu m, Input i, Log l, Structures.RawImage image, Guid currentGuid)
        {
            _menu = m;
            _input = i;
            _log = l;
            _image = image;
            _runGuid = currentGuid;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        private void ShowDialog()
        {
            _menu.ClearUserSection();
            _menu.WriteLine("You have selected");
        }

        public double[,] Result() => _workingArray;
    }
}
