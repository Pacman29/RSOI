using System;
using System.Collections.Generic;
using System.Text;

namespace DataBaseServer.Exceptions.DBExceptions
{
    [Serializable()]
    public class UpdateException : Exception
    {
        public UpdateException() { }
        public UpdateException(string message) : base(message) { }
        public UpdateException(string message, Exception inner) : base(message, inner) { }

        protected UpdateException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
