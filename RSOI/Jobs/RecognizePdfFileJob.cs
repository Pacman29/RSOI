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
        private readonly Recognize.RecognizeClient _recognizeClient;
        private int[] _pages;

        public RecognizePdfFileJob(Guid jobId,Recognize.RecognizeClient recognizeClient, byte[] pdfFile, int[] pages, BaseJob rootJob = null)
        {
            this._pdfFile = pdfFile;
            this._recognizeClient = recognizeClient;
            this.Guid = jobId;
            this.RootJob = rootJob;
            this._pages = pages;
        }
        
        public override async Task ExecuteAsync()
        {
            var guid = this.Guid;
            if (guid == null)
                throw new Exception("job guid is null");
            var pdfFile = new PdfFile()
            {
                Bytes = ByteString.CopyFrom(_pdfFile),
                JobId = ((Guid) guid).ToString(),
                Pages = { _pages}
            };
            await _recognizeClient.RecognizePdfAsync(pdfFile);
        }

        public override async Task Reject()
        {
            Console.WriteLine($"{this.Guid} job reject");
        }

        public override void OnDone()
        {
            using (var compressedFileStream = new FileStream("zip.zip", FileMode.Create))
            {
                compressedFileStream.Write(this.Bytes);
            }

            if(this.Guid != null)
                Task.Run(async () =>
                {
                    _recognizeClient.DoneJobCall(new DoneJob()
                    {
                        JobId = this.Guid.ToString()
                    });
                });
        }
    }
}