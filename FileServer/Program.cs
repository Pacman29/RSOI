using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Core.Logging;
using GRPCService;

namespace FileServer
{
    public class Program
    {
        private static Server server = null;
        
        static void Main(string[] args)
        {
            server = GrpcServerCreator.Create("0.0.0.0", 8082, "FileServer", "FileServerPassword",
                GRPCService.GRPCProto.FileServer.BindService(new FileServerGrpc()));
            server.Start();
            Console.WriteLine("FileServer listening on port " + 8082);
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