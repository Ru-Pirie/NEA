using System;
using System.Runtime.Serialization;

namespace BackendLib.Exceptions
{
    [Serializable]
    public class KernelException : Exception
    {
        public KernelException()
        {
        }

        public KernelException(string message) : base(message)
        {
        }

        public KernelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KernelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}