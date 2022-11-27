using System;
using System.Runtime.Serialization;

namespace BackendLib.Exceptions
{
    [Serializable]
<<<<<<< HEAD
    public class LoggerException : Exception
=======
    internal class LoggerException : Exception
>>>>>>> b7ce23b3c76b600cde0fa745e3e3a87201b3e798
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
