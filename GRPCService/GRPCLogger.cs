using Grpc.Core.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRPCService
{
    public class GRPCLogger : ILogger
    {
        public void Debug(string message)
        {
            throw new NotImplementedException();
        }

        public void Debug(string format, params object[] formatArgs)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Error(string format, params object[] formatArgs)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception exception, string message)
        {
            throw new NotImplementedException();
        }

        public ILogger ForType<T>()
        {
            throw new NotImplementedException();
        }

        public void Info(string message)
        {
            throw new NotImplementedException();
        }

        public void Info(string format, params object[] formatArgs)
        {
            throw new NotImplementedException();
        }

        public void Warning(string message)
        {
            throw new NotImplementedException();
        }

        public void Warning(string format, params object[] formatArgs)
        {
            throw new NotImplementedException();
        }

        public void Warning(Exception exception, string message)
        {
            throw new NotImplementedException();
        }
    }
}
