using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class DeleteFileInfoFromDatabaseJob : GateWayJob<bool>
    {
        private readonly string _jobId;
        private readonly int _id;
        private readonly string _path;
        private readonly EnumFileType _fileType;
        public IDataBaseService DataBaseService { get; set; }
        
        public DeleteFileInfoFromDatabaseJob(string jobId, int id, string path, EnumFileType fileType)
        {
            _jobId = jobId;
            _id = id;
            _path = path;
            _fileType = fileType;
            
            this.OnDone += async job =>
            {
                this.InvokeOnHaveResult(BytesDeserializer<bool>.Deserialize(job));
            };
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.DeleteFileInfo(new FileInfo()
            {
                FileType = this._fileType,
                JobId = this._jobId,
                Path = _path,
                Id = _id
            });
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}