using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class RecognizePdfFileJob : GateWayJob
    {
        private readonly byte[] _pdfFile;
        public IRecognizeService RecognizeService { get; set; }
        private readonly List<int> _pages;

        public RecognizePdfFileJob(byte[] pdfFile, List<int> pages)
        {
            this._pdfFile = pdfFile;
            this._pages = pages;
        }
        
        public override async Task ExecuteAsync()
        {
            var pdfFile = new PdfFile()
            {
                Bytes = ByteString.CopyFrom(_pdfFile),
                Pages = {_pages}
            };
            var jobInfo = await RecognizeService.RecognizePdf(pdfFile);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }

        public override async Task Reject()
        {
            Console.WriteLine($"{this.Guid} job reject");
        }
    }
}