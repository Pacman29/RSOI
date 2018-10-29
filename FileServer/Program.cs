using System;
using Grpc.Core;

namespace FileServer
{
    public class Program
    {
        private static Server server = null;
        
        static void Main(string[] args)
        {
            server = new Server
            {
                Services = {GRPCService.GRPCProto.FileServer.BindService(new FileServerGrpc())},
                Ports = {new ServerPort("0.0.0.0", 8082, ServerCredentials.Insecure)}
            };
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