using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RSOI;

namespace RSOI_Gateway
{
    public class Program
    {

        public class AspServer : IDisposable
        {
            private  IWebHost aspNet;
            public AspServer(string[] args)
            {
                aspNet = CreateWebHostBuilder(args).Build();
            }

            private IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>();
            
            public async Task Run()
            {
                aspNet?.RunAsync();
            }
            
            public void Dispose()
            {
                aspNet?.StopAsync().Wait();
                Console.WriteLine("ASP Net Server stop");
            }

            public void Deconstruct()
            {
                Dispose();
            }
        }

        public class NotifyGrpcServer : IDisposable
        {
            private  Server grpcServer;

            public NotifyGrpcServer(string host,int port)
            {
                grpcServer = new Server
                {
                    Services = {GateWay.BindService(new GateWayServerGrpc())},
                    Ports = {new ServerPort(host, port, ServerCredentials.Insecure)}
                };
                Console.WriteLine("GRPC Server listening on port " + port);
            }

            public async Task Run()
            {
                grpcServer?.Start();
            }
            
            public void Dispose()
            {
                grpcServer?.ShutdownAsync().Wait();
                Console.WriteLine("NotifyServer stop");
            }

            public void Deconstruct()
            {
                Dispose();
            }
        }

        private static NotifyGrpcServer _notifyGrpcServer;
        private static AspServer _aspServer;
        
        public static async Task Main(string[] args)
        {
            _notifyGrpcServer = new NotifyGrpcServer("0.0.0.0",8001);
            _aspServer = new AspServer(args);

            _notifyGrpcServer.Run();
            _aspServer.Run();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            while (true)
                Console.ReadKey(true);
        }
        
        static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            _notifyGrpcServer?.Dispose();
            _aspServer?.Dispose();
        }
    }
}