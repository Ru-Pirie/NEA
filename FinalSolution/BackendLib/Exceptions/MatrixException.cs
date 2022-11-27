using System;
using System.Runtime.Serialization;

namespace BackendLib.Exceptions
{
    [Serializable]
<<<<<<< HEAD
    public class MatrixException : Exception
=======
    internal class MatrixException : Exception
>>>>>>> b7ce23b3c76b600cde0fa745e3e3a87201b3e798
    {
        public MatrixException()
        {
        }

        public MatrixException(string message) : base(message)
        {
        }

        public MatrixException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MatrixException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
