using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using JobExecutor;
using PDFiumSharp;

namespace RecognizePdfServer.Jobs
{
    public class RecognizePdfJob : BaseJob
    {
        private readonly MemoryStream _stream;
        private readonly int[] _pages;

        public RecognizePdfJob(MemoryStream stream, int[] pages)
        {
            _stream = stream;
            _pages = pages;
        }

        private void RecognizePage(ZipArchive archive, PdfPage page)
        {
            var entry = archive.CreateEntry($"{page.Index}.jpg");
            using (var bitmap = new PDFiumBitmap((int) page.Width, (int) page.Height, true))
            using (var entryStream = entry.Open())
            {
                page.Render(bitmap);
                bitmap.Save(entryStream);
            }
        }
        
        public override async Task ExecuteAsync()
        {
            try
            {
                var zipMemoryStream = new MemoryStream();

                using (var archive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                using (var doc = new PdfDocument(_stream.ToArray()))
                {
                    if (_pages.Length == 0)
                        foreach (var page in doc.Pages)
                            RecognizePage(archive,page);
                    else
                        foreach (var pageIdx in _pages)
                            RecognizePage(archive,doc.Pages[pageIdx]);                  
                }
                this.Bytes = zipMemoryStream.ToArray();
            } catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
            
        }

        public override async Task Reject()
        {
            
        }
    }
}