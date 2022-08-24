using System;

namespace BackendLib.Exceptions
{
    public class LoggerException : Exception
    {
        public LoggerException(string message) : base(message)
        {
        }
    }
}
