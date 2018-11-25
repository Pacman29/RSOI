using System.Collections.Generic;
using AuthOptions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Core.Logging;
using GRPCService.GRPCProto;
using AuthOptions = AuthOptions.AuthOptions;

namespace GRPCService
{
    public static class GrpcServerCreator
    {
        public static Server Create(string host, int port, string serverName, string password, ServerServiceDefinition service)
        {
            GrpcEnvironment.SetLogger(new ConsoleLogger());
            var channelOptions = new List<ChannelOption>
            {
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1)
            };

            var jwtTokenGenerator = new JwtTokenGenerator(new global::AuthOptions.AuthOptions()
            {
                ISSUER = serverName,
                AUDIENCE = "Gateway"
            });
            
            var authService = Authorize.BindService(new AuthServerGrpc(serverName,password,jwtTokenGenerator));
            
            service = service.Intercept(new AuthServerInterceptor(jwtTokenGenerator));
            
            var server = new Server(channelOptions)
            {
                Services = {service,authService},
                Ports = {new ServerPort(host, port, ServerCredentials.Insecure)}
            };
            
            return server;
        }
    }
}