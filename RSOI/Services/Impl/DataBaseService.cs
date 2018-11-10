using System;
using System.Collections.Generic;
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
            var channelOptions = new List<ChannelOption>();
            //channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, 1024*1024*1024));
            channel = new Channel(databaseUri, ChannelCredentials.Insecure,channelOptions);
            client = new DataBase.DataBaseClient(channel);
        }

        public async Task<JobInfo> CreateFileInfo(FileInfo fileInfo)
        {
            return await client.SaveFileInfoAsync(fileInfo);
        }
        
        public async Task<JobInfo> UpdateOrCreateJob(JobInfo jobInfo)
        {
            return await client.UpdateOrCreateJobAsync(jobInfo);
        }

        public async Task DoneJob(Guid jobId)
        {
            var result = await client.DoneJobCallAsync(new DoneJob()
            {
                JobId = jobId.ToString()
            });
        }

        public async Task<JobInfo>  GetJobInfo(string jobId)
        {
            return await client.GetJobInfoAsync(new JobInfo()
            {
                JobId = jobId.ToString()
            });
        }

        public async Task<JobInfo> ImagesInfo(string jobId, long firstPageNo, long count)
        {
            return await client.GetImagesInfoAsync(new ImagesInfo()
            {
                Count = count,
                FirstPageNo = firstPageNo,
                JobId = jobId
            });
        }

        public async void Dispose()
        {
            await channel.ShutdownAsync();
        }
    }
}