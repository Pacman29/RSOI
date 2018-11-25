using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService;
using GRPCService.GRPCProto;

namespace RSOI.Services.Impl
{
    public class DataBaseService : BaseGrpcService, IDataBaseService
    {
        private readonly DataBase.DataBaseClient _client;

        public DataBaseService(string databaseUri)
        {
            var channelOptions = new List<ChannelOption>
            {
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1)
            };

            AuthHandler = new AuthorizationHandler("DataBase","DataBasePassword",databaseUri,ChannelCredentials.Insecure, channelOptions);
            var channel = GrpsClientChannelCreator.Create(databaseUri, ChannelCredentials.Insecure, channelOptions,AuthHandler.GetAuthorizeFunc());
            _client = new DataBase.DataBaseClient(channel);
        }

        public async Task<JobInfo> CreateFileInfo(FileInfo fileInfo)
        {
            return await _client.SaveFileInfoAsync(fileInfo,GetMetadata());
        }
        
        public async Task<JobInfo> UpdateOrCreateJob(JobInfo jobInfo)
        {
            return await _client.UpdateOrCreateJobAsync(jobInfo,GetMetadata());
        }

        public async Task DoneJob(Guid jobId)
        {
            var result = await _client.DoneJobCallAsync(new DoneJob()
            {
                JobId = jobId.ToString()
            },GetMetadata());
        }

        public async Task<JobInfo>  GetJobInfo(string jobId)
        {
            return await _client.GetJobInfoAsync(new JobInfo()
            {
                JobId = jobId.ToString()
            },GetMetadata());
        }

        public async Task<JobInfo> ImagesInfo(string jobId, long firstPageNo, long count)
        {
            return await _client.GetImagesInfoAsync(new ImagesInfo()
            {
                Count = count,
                FirstPageNo = firstPageNo,
                JobId = jobId
            },GetMetadata());
        }

        public async Task<JobInfo> DeleteJobInfo(string jobId)
        {
            return await _client.DeleteJobInfoAsync(new JobInfo()
            {
                JobId = jobId.ToString()
            },GetMetadata());
        }

        public async Task<JobInfo> DeleteFileInfo(FileInfo pdfFileInfo)
        {
            return await _client.DeleteFileInfoAsync(pdfFileInfo,GetMetadata());
        }

        public async Task<JobInfo> GetAllJobInfos()
        {
            return await _client.GetAllJobInfosAsync(new Empty(),GetMetadata());
        }

        public async void Dispose()
        {
            //await channel.ShutdownAsync();
        }
    }
}