using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using JobExecutor;
using PDFiumSharp;

namespace RecognizePdfServer.Jobs
{
    public class RecognizePdfJob : BaseJob
    {
        private readonly MemoryStream _stream;
        private readonly long _fileLength;
        private readonly string _fileName;

        public RecognizePdfJob(MemoryStream stream, long fileLength, string fileName)
        {
            _stream = stream;
            _fileLength = fileLength;
            _fileName = fileName;
        }

        public override async Task Execute()
        {

            PDFiumSharp.PDFium();
            using (var doc = new PdfDocument(this._stream))
            {
                foreach (var page in doc.Pages)
                {
                    Console.WriteLine(page);
                }
            }
        }

        public override async Task Reject()
        {
            
        }
    }
}