using BackendLib;
using BackendLib.Interfaces;
using LocalApp.CLI;
using System;

namespace LocalApp
{
    // Use inheritance?
    internal class SyncEdgeDetection : IHandler
    {
        private readonly Menu m;
        private readonly Input i;
        private readonly Log l;
        private readonly Structures.RawImage image;
        private readonly Guid runGuid;
        private double[,] _workingArray;

        public SyncEdgeDetection(Menu m, Input i, Log l, Structures.RawImage image, Guid currentGuid)
        {
            this.m = m;
            this.i = i;
            this.l = l;
            this.image = image;
            this.runGuid = currentGuid;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public double[,] Result() => _workingArray;
    }
}
