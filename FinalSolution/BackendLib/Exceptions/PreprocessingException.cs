using System;
using System.Runtime.Serialization;
<<<<<<< HEAD

namespace BackendLib.Exceptions
{
    [Serializable]
    public class PreprocessingException : Exception
=======
namespace BackendLib.Exceptions
{
    [Serializable]
    internal class PreprocessingException : Exception
>>>>>>> b7ce23b3c76b600cde0fa745e3e3a87201b3e798
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
