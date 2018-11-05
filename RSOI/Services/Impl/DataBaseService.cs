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

        public async Task<JobInfo> CreatePdfFile(PdfFileInfo pdfFileInfo)
        {
            return await client.SavePdfFileInfoAsync(pdfFileInfo);
        }

        public async Task DoneJob(Guid jobId)
        {
            var result = await client.DoneJobCallAsync(new DoneJob()
            {
                JobId = jobId.ToString()
            });
        }
        
        public async void Dispose()
        {
            await channel.ShutdownAsync();
        }
    }
}