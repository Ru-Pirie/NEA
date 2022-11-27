using System;
using System.Runtime.Serialization;

namespace BackendLib.Exceptions
{
    [Serializable]
<<<<<<< HEAD
    public class KernelException : Exception
=======
    internal class KernelException : Exception
>>>>>>> b7ce23b3c76b600cde0fa745e3e3a87201b3e798
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