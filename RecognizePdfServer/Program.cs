using System;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace RecognizePdfServer
{
    public class Program
    {
        private static Server server = null;
        
        static void Main(string[] args)
        {
            server = new Server
            {
                Services = {RecognizeService.BindService(new PdfRecognizeServerGrpc())},
                Ports = {new ServerPort("0.0.0.0", 8081, ServerCredentials.Insecure)}
            };
            server.Start();
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