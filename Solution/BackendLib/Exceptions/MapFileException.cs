using System;
using System.Runtime.Serialization;
namespace BackendLib.Exceptions
{
    [Serializable]

    public class MapFileException : Exception
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