using System;
#if !DOTNET5_4
using System.Runtime.Serialization;
#endif

namespace DeKreyConsulting.AdoTestability
{
#if !DOTNET5_4
    [Serializable]
#endif
    internal class BadExecutionPlanException : Exception
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

#if !DOTNET5_4
        protected BadExecutionPlanException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}