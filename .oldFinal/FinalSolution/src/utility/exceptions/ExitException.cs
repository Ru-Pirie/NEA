using System;
using System.Runtime.Serialization;

namespace FinalSolution.src.local
{
    [Serializable]
    internal class ExitException : Exception
    {
        public ExitException()
        {
        }

        public ExitException(string message) : base(message)
        {
        }

        public ExitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}