using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class AddPdfToDatabaseJob : BaseJob
    {
        private PdfFileInfo _fileInfo;
        private IDataBaseService _dataBaseService;

        public AddPdfToDatabaseJob(Guid jobId,IDataBaseService dataBaseService, PdfFileInfo fileInfo, BaseJob rootJob = null)
        {
            this._fileInfo = fileInfo;
            this._dataBaseService = dataBaseService;
            this.Guid = jobId;
            this.RootJob = rootJob;
        }
        
        public override async Task ExecuteAsync()
        {
            var guid = this.Guid;
            if (guid == null)
                throw new Exception("job guid is null");
            _fileInfo.JobId = ((Guid) guid).ToString();
            await _dataBaseService.CreatePdfFile(_fileInfo);
        }

        public override async Task Reject()
        {
            Console.WriteLine($"{this.Guid} job reject");
        }

        public override void OnDone()
        {
            if(this.Guid != null)
                Task.Run(async () =>
                {
                    _dataBaseService.DoneJob((Guid) this.Guid);
                });
        }
    }
}