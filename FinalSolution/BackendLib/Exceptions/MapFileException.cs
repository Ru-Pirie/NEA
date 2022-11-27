using System;
using System.Runtime.Serialization;
namespace BackendLib.Exceptions
{
    [Serializable]
<<<<<<< HEAD
    public class MapFileException : Exception
=======
    internal class MapFileException : Exception
>>>>>>> b7ce23b3c76b600cde0fa745e3e3a87201b3e798
    {
        public MapFileException()
        {
        }

        public MapFileException(string message) : base(message)
        {
        }

        public MapFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MapFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}