using System.Collections.Generic;
using Grpc.Core;
using Grpc.Core.Logging;
using GRPCService.GRPCProto;

namespace GRPCService
{
    public class GrpcServerCreator
    {
        public static Server Create(string host, int port, ServerServiceDefinition service)
        {
            GrpcEnvironment.SetLogger(new ConsoleLogger());
            var channelOptions = new List<ChannelOption>();
            channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1));
            
            var server = new Server(channelOptions)
            {
                Services = {service},
                Ports = {new ServerPort(host, port, ServerCredentials.Insecure)}
            };
            return server;
        }
    }
}