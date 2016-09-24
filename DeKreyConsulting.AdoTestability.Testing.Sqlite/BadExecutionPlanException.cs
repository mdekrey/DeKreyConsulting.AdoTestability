using System;
#if NET451
using System.Runtime.Serialization;
#endif

namespace DeKreyConsulting.AdoTestability.Testing.Sqlite
{
#if NET451
    [Serializable]
#endif
    public class BadExecutionPlanException : Exception
    {
        public BadExecutionPlanException()
        {
        }

        public BadExecutionPlanException(string message) : base(message)
        {
        }

        public BadExecutionPlanException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NET451
        protected BadExecutionPlanException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}