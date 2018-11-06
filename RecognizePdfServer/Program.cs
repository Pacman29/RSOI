using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Core.Logging;
using GRPCService.GRPCProto;

namespace RecognizePdfServer
{
    public class Program
    {
        private static Server server = null;
        
        static void Main(string[] args)
        {
            GrpcEnvironment.SetLogger(new TextWriterLogger(Console.Out));
            var channelOptions = new List<ChannelOption>();
            channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1));

            server = new Server(channelOptions)
            {
                Services = {Recognize.BindService(new PdfRecognizeServerGrpc())},
                Ports = {new ServerPort("0.0.0.0", 8081, ServerCredentials.Insecure)}
            };
            
            Console.WriteLine("PdfServer listening on port " + 8081);
            Console.WriteLine("Press Ctrl+C  to stop the server...");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            while (true)
                Console.ReadKey(true);
        }
        
        static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            server?.ShutdownAsync().Wait();
        }
    }
}