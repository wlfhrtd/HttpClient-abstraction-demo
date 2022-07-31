using System;


namespace Movies.Client.Exceptions
{

    [Serializable]
    public class UnauthorizedApiAccessException : Exception
    {
        public UnauthorizedApiAccessException() { }

        public UnauthorizedApiAccessException(string message) : base(message) { }

        public UnauthorizedApiAccessException(string message, Exception inner) : base(message, inner) { }

        protected UnauthorizedApiAccessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
