using System;
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

            var cont = continuation(request, context);
            var response = cont.ResponseHeadersAsync.Result;
            if (cont.GetStatus().StatusCode != StatusCode.Cancelled) 
                return cont;
            var authResult = _authorize().Result;
            if (authResult != "")
            {
                context.SetAccessToken(authResult);
                return continuation(request, context);
            }
            else
            {
                return cont;
            }
        }
    }
}