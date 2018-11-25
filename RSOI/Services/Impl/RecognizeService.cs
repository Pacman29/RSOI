using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService;
using GRPCService.GRPCProto;

namespace RSOI.Services.Impl
{
    public class RecognizeService : BaseGrpcService, IRecognizeService
    {
        private readonly Recognize.RecognizeClient _client;

        public RecognizeService(string recognizeUri)
        {
            var channelOptions = new List<ChannelOption>
            {
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1)
            };
            AuthHandler = new AuthorizationHandler("RecognizeServer","RecognizeServerPassword",recognizeUri,ChannelCredentials.Insecure, channelOptions);
            var channel = GrpsClientChannelCreator.Create(recognizeUri, ChannelCredentials.Insecure, channelOptions,AuthHandler.GetAuthorizeFunc());
            _client = new Recognize.RecognizeClient(channel);
        }

        public async Task<JobInfo> RecognizePdf(PdfFile pdfFile)
        {
            JobInfo result = null;
            try
            {
                Console.WriteLine("Recognize PDF");
                result =  await _client.RecognizePdfAsync(pdfFile,GetMetadata());
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return result;
        }
    }
}