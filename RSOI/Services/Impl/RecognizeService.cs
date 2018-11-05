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
            _channel = new Channel(recognizeUri, ChannelCredentials.Insecure);
            Client = new Recognize.RecognizeClient(_channel);
        }

        public async Task<JobInfo> RecognizePdf(PdfFile pdfFile)
        {
            return await Client.RecognizePdfAsync(pdfFile);
        }
    }
}