using System;
using System.Threading.Tasks;
using JobExecutor;
using Models.Responses;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class GetImagesInfoJob : GateWayJob<ImagesResponseModel>
    {
        private readonly string _jobId;
        private readonly long _firstPageNo;
        private readonly long _count;
        public IDataBaseService DataBaseService { get; set; }
        
        public GetImagesInfoJob(string jobId, long firstPageNo, long count)
        {
            _jobId = jobId;
            _firstPageNo = firstPageNo;
            _count = count;
            
            this.OnDone += async job =>
            {
                this.InvokeOnHaveResult(BytesDeserializer<ImagesResponseModel>.Deserialize(job));
            };
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.ImagesInfo(this._jobId,this._firstPageNo,this._count);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}