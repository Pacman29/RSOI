using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GRPCService
{
    public static class ClientInterceptorContextExtension
    {
        public static void SetAccessToken<TRequest, TResponse>(this ClientInterceptorContext<TRequest, TResponse> context, string token) where TRequest : class where TResponse : class
        {
            var metadata = context.Options.Headers ?? new Metadata();
            metadata.Add("Authorization",$"Bearer {token}");
        }
    }
}