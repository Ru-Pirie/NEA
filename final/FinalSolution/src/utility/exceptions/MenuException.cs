using System;
using System.Runtime.Serialization;

namespace FinalSolution.src.utility
{
    [Serializable]
    internal class MenuException : Exception
    {
        public MenuException()
        {
        }

        public MenuException(string message) : base(message)
        {
        }

        public MenuException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MenuException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}