using System;
using System.Runtime.Serialization;

namespace BackendLib.Exceptions
{
    [Serializable]

    public class LoggerException : Exception

    {
        public LoggerException()
        {
        }

        public LoggerException(string message) : base(message)
        {
        }

        public LoggerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LoggerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
