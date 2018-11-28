using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService;
using GRPCService.GRPCProto;

namespace RSOI.Services.Impl
{
    public class FileService : BaseGrpcService, IFileService
    {
        private readonly FileServer.FileServerClient _client;


        public FileService(string fileServerUri)
        {
            var channelOptions = new List<ChannelOption>
            {
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1)
            };
            AuthHandler = new AuthorizationHandler("FileServer","FileServerPassword",fileServerUri,ChannelCredentials.Insecure, channelOptions);
            var channel = GrpsClientChannelCreator.Create(fileServerUri, ChannelCredentials.Insecure, channelOptions,AuthHandler.GetAuthorizeFunc());
            _client = new FileServer.FileServerClient(channel);
        }
        
        public async Task<JobInfo> SaveFile(File file)
        {
            return await _client.SaveFileAsync(file,GetMetadata());
        }

        public async Task<JobInfo> GetFile(Path path)
        {
            return await _client.GetFileAsync(path,GetMetadata());
        }
        
        public async Task<JobInfo> GetFiles(IEnumerable<string> paths)
        {
            return await _client.GetFilesAsync(new Paths()
            {
                FilePaths = {paths}
            },GetMetadata());
        }

        public async Task<JobInfo> DeleteFile(string path)
        {
            return await _client.DeleteFileAsync(new Path() {
                Path_ = path
            },GetMetadata());
        }
    }
}