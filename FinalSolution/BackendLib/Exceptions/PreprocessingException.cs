using System;
using System.Runtime.Serialization;
namespace BackendLib.Exceptions
{
    [Serializable]
    internal class PreprocessingException : Exception
    {
        public PreprocessingException()
        {
        }

        public PreprocessingException(string message) : base(message)
        {
        }

        public PreprocessingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PreprocessingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
