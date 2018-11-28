using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Core.Logging;
using GRPCService;
using GRPCService.GRPCProto;

namespace DataBaseServer
{
    public class Program
    {
        private static Server server = null;
        
        static void Main(string[] args)
        {
            server = GrpcServerCreator.Create("0.0.0.0", 8080, "DataBase", "DataBasePassword", DataBase.BindService(new DataBaseServerGrpc()));
            server.Start();
            Console.WriteLine("DataBase server listening on port " + 8080);
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