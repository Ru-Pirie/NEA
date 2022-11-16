using System;

namespace BackendLib.Exceptions
{
    public class MapFileException : Exception
    {
        public MapFileException(string message) : base(message)
        {
        }
    }
}