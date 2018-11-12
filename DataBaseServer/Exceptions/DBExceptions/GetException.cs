using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DataBaseServer.Exceptions.DBExceptions
{
    class GetException : Exception
    {
        public GetException() { }
        public GetException(string message) : base(message) { }
        public GetException(string message, Exception inner) : base(message, inner) { }

        protected GetException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
    
}
