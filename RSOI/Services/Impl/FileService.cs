using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace RSOI.Services.Impl
{
    public class FileService : IFileService
    {
        private readonly Channel _channel;
        private FileServer.FileServerClient Client { get; set; }
        
        public FileService(string fileServerUri)
        {
            var channelOptions = new List<ChannelOption>();
            channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1));
            _channel = new Channel(fileServerUri, ChannelCredentials.Insecure, channelOptions);
            Client = new FileServer.FileServerClient(_channel);
        }
        
        public async Task<JobInfo> SaveFile(File file)
        {
            return await Client.SaveFileAsync(file);
        }

        public async Task<JobInfo> GetFile(Path path)
        {
            return await Client.GetFileAsync(path);
        }
    }
}