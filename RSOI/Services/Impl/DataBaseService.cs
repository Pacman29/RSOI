using System;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace RSOI.Services.Impl
{
    public class DataBaseService : IDataBaseService
    {
        private readonly Channel channel;
        private readonly DataBase.DataBaseClient client;
        public DataBaseService(string databaseUri)
        {
            channel = new Channel(databaseUri, ChannelCredentials.Insecure);
            client = new DataBase.DataBaseClient(channel);
        }

        public async Task CreatePdfFile(PdfFileInfo pdfFileInfo)
        {
            var result = await client.SavePdfFileInfoAsync(pdfFileInfo);
            if (result.JobStatus != EnumJobStatus.Execute)
                throw new Exception("not execute");
        } 

        public async void Dispose()
        {
            await channel.ShutdownAsync();
        }
    }
}