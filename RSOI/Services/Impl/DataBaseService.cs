using System;
using Grpc.Core;
using GRPCService.DataBaseProto;

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

        public async void CreatePdfFile(PdfFileInfo pdfFileInfo)
        {
            var result = await client.SavePdfFileInfoAsync(pdfFileInfo);
            if (result.Error.IsError)
                throw new Exception(result.Error.ErrorMessage);
        } 

        public async void Dispose()
        {
            await channel.ShutdownAsync();
        }
    }
}