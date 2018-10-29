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
        private IFileService _fileService;
        private string _path;

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
            await _fileService.SaveFile(new File()
            {
                Bytes = ByteString.CopyFrom(this.Bytes),
                FilePath = new Path()
                {
                    JobId = this.Guid.ToString(),
                    Path_ = _path
                }
            });
        }

        public override Task Reject()
        {
            return base.Reject();
        }
    }
}