using System;
using System.Threading.Tasks;
using Google.Protobuf;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class SavePdfFileJob : BaseJob
    {
        private readonly IFileService _fileService;
        private readonly string _path;

        public SavePdfFileJob(Guid jobId, IFileService fileService, byte[] bytes, string path, BaseJob root = null)
        {
            this.Guid = jobId;
            this.RootJob = root;
            this._fileService = fileService;
            this.Bytes = bytes;
            this._path = path;
        }


        public override async Task ExecuteAsync()
        {
            var file = new File()
            {
                Bytes = ByteString.CopyFrom(this.Bytes),
                FilePath = new Path()
                {
                    Path_ = _path
                }
            };
            var jobInfo = await _fileService.SaveFile(file);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }

        public override Task Reject()
        {
            return base.Reject();
        }
    }
}