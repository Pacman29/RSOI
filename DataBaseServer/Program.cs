using System;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace DataBaseServer
{
    class Program
    {
        private static Server server = null;
        
        static void Main(string[] args)
        {
            
            server = new Server
            {
                Services = {DataBase.BindService(new DataBaseServerGrpc())},
                Ports = {new ServerPort("0.0.0.0", 8080, ServerCredentials.Insecure)}
            };
            server.Start();
            Console.WriteLine("Server listening on port " + 8080);
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