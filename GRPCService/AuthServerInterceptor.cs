using System;
using System.Linq;
using System.Threading.Tasks;
using AuthOptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GRPCService
{
    public class AuthServerInterceptor : Interceptor
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthServerInterceptor(IJwtTokenGenerator jwtTokenGenerator)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request
            , ServerCallContext context
            , UnaryServerMethod<TRequest, TResponse> continuation
        )
        {
            if (!_jwtTokenGenerator.ValidateToken(context.GetAccessToken()))
            {
                //context.Status = new Status(StatusCode.Unauthenticated, "Invalid token");
                //return default(TResponse);
                throw new RpcException(new Status(StatusCode.Unauthenticated,"Invalid token"));
            }
            else
            {
                return await continuation(request, context);
            }
        }
        
    }
}