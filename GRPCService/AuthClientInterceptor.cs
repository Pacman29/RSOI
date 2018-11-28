using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GRPCService
{
    public class AuthClientInterceptor : Interceptor
    {
        private readonly Func<Task<string>> _authorize;

        public AuthClientInterceptor(Func<Task<string>> authorize)
        {
            _authorize = authorize;
        }
        
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var cont = default(AsyncUnaryCall<TResponse>); 
            try
            {
                cont = continuation(request, context);
                var response = cont.ResponseAsync.Result;
                var headers = cont.ResponseHeadersAsync.Result;
                var res = cont.GetAwaiter().GetResult();
                return cont;
            }
            catch (AggregateException e)
            {
                var exception = (RpcException) e.InnerExceptions.FirstOrDefault(err =>
                {
                    if (err.GetType() != typeof(RpcException)) return false;
                    var rpcErr = (RpcException) err;
                    return rpcErr.StatusCode == StatusCode.Unauthenticated;
                });
                if (exception == null) return cont;
                var authResult = _authorize().Result;
                if (authResult == "") return cont;
                context.SetAccessToken(authResult);
                cont = continuation(request, context);
            }
            return cont;
        }
    }
}