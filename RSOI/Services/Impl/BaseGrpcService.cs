using Grpc.Core;
using GRPCService;

namespace RSOI.Services.Impl
{
    public class BaseGrpcService
    {
        protected AuthorizationHandler AuthHandler;

        protected Metadata GetMetadata()
        {
            var m = new Metadata();
            var token = AuthHandler.Token;
            return token == "" ? m : m.SetAccessToken(token);
        }
    }
}