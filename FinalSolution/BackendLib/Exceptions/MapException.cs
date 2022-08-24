using System;

namespace BackendLib.Exceptions
{
    public class MapException : Exception
    {
        public MapException(string message) : base(message)
        {
        }
    }
}