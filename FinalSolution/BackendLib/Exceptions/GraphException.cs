using System;

namespace BackendLib.Exceptions
{
    public class GraphException : Exception
    {
        public GraphException(string message) : base(message)
        {
        }
    }
}
