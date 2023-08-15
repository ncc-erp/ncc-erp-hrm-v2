using System;
using System.Runtime.Serialization;

namespace HRMv2.Authorization
{
    [Serializable]
    internal class InvalidJwtException : Exception
    {
        public InvalidJwtException()
        {
        }

        public InvalidJwtException(string message) : base(message)
        {
        }

        public InvalidJwtException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidJwtException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}