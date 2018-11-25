﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Grpc.Core;
using GRPCService;
using GRPCService.GRPCProto;
using File = System.IO.File;
using FileInfo = System.IO.FileInfo;
using Path = System.IO.Path;

namespace RecognizePdfServer
{
    public class Program
    {
        private static Server server = null;
        
        
        static void Main(string[] args)
        {
            server = GrpcServerCreator.Create("0.0.0.0", 8081, "RecognizeServer", "RecognizeServerPassword",
                Recognize.BindService(new PdfRecognizeServerGrpc()));
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