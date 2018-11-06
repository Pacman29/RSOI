using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace RSOI.Services.Impl
{
    public class RecognizeService : IRecognizeService
    {
        private readonly Channel _channel;
        private Recognize.RecognizeClient Client { get; set; }
        
        public RecognizeService(string recognizeUri)
        {
            var channelOptions = new List<ChannelOption>();
            channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1));
            _channel = new Channel(recognizeUri, ChannelCredentials.Insecure,channelOptions);
            Client = new Recognize.RecognizeClient(_channel);
        }

        public async Task<JobInfo> RecognizePdf(PdfFile pdfFile)
        {
            JobInfo result = null;
            try
            {
                Console.WriteLine("Recognize PDF");
                result =  await Client.RecognizePdfAsync(pdfFile);
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return result;
        }
    }
}