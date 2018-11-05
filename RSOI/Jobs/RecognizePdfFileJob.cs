using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Google.Protobuf;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class RecognizePdfFileJob : BaseJob
    {
        private readonly byte[] _pdfFile;
        private readonly IRecognizeService _recognizeService;
        private readonly int[] _pages;

        public RecognizePdfFileJob(Guid jobId,IRecognizeService recognizeService, byte[] pdfFile, int[] pages, BaseJob rootJob = null)
        {
            this._pdfFile = pdfFile;
            this._recognizeService = recognizeService;
            this.Guid = jobId;
            this.RootJob = rootJob;
            this._pages = pages;
        }
        
        public override async Task ExecuteAsync()
        {
            var pdfFile = new PdfFile()
            {
                Bytes = ByteString.CopyFrom(_pdfFile),
                Pages = { _pages}
            };
            var jobInfo = await _recognizeService.RecognizePdf(pdfFile);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }

        public override async Task Reject()
        {
            Console.WriteLine($"{this.Guid} job reject");
        }
    }
}