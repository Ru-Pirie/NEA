using System;
using System.Runtime.Serialization;

namespace BackendLib.Exceptions
{
    [Serializable]
<<<<<<< HEAD
    public class GraphException : Exception
=======
    internal class GraphException : Exception
>>>>>>> b7ce23b3c76b600cde0fa745e3e3a87201b3e798
    {
        public GraphException()
        {
        }

        public GraphException(string message) : base(message)
        {
        }

        public GraphException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GraphException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
