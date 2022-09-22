using BackendLib;
using BackendLib.Interfaces;
using LocalApp.CLI;
using System;

namespace LocalApp
{
    // Use inheritance?
    internal class SyncEdgeDetection : IHandler
    {
        private Menu m;
        private Input i;
        private Log l;
        private Structures.RawImage image;
        private double[,] _workingArray;

        public SyncEdgeDetection(Menu m, Input i, Log l, Structures.RawImage image)
        {
            this.m = m;
            this.i = i;
            this.l = l;
            this.image = image;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public double[,] Result() => _workingArray;
    }
}
