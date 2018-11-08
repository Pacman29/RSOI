using System;
using System.Threading.Tasks;
using Google.Protobuf;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;
using PdfFile = Models.Requests.PdfFile;

namespace RSOI.Jobs
{
    public class SaveFileJob : GateWayJob
    {
        public IFileService FileService { get; set; }
        private readonly string _path;
        private readonly byte[] _bytes;

        public SaveFileJob(byte[] bytes, string path)
        {
            this._path = path;
            this._bytes = bytes;
        }

        public override async Task ExecuteAsync()
        {
            var file = new File()
            {
                Bytes = ByteString.CopyFrom(this._bytes),
                FilePath = new Path()
                {
                    Path_ = _path
                }
            };
            var jobInfo = await FileService.SaveFile(file);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }

        public override Task Reject()
        {
            return base.Reject();
        }
    }
}