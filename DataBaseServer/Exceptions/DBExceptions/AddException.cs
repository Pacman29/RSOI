using System;

namespace DataBaseServer.Exceptions.DBExceptions
{
    [Serializable()]
    public class AddException : Exception
    {
        public AddException(){}
        public AddException(string message) : base(message) { }
        public AddException(string message, Exception inner) : base(message, inner) { }
        
        protected AddException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}