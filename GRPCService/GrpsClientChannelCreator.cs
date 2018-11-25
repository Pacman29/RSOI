using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using GRPCService.GRPCProto;

namespace GRPCService
{
    public static class GrpsClientChannelCreator
    {
        public static CallInvoker Create(
            string target, 
            ChannelCredentials credentials, 
            IEnumerable<ChannelOption> options, 
            Func<Task<string>> autorizeFunc)
        {
            var channel =  new Channel(target, credentials, options);
            
            var invoker = channel.Intercept(new AuthClientInterceptor(autorizeFunc));
            return invoker;
        }
    }
}