using System.Threading.Tasks;
using JobExecutor;
using PDFiumSharp;

namespace RecognizePdfServer.Jobs
{
    public class RecognizePageJob : BaseJob
    {
        private readonly PdfPage _pdfPage;

        public RecognizePageJob(PdfPage pdfPage)
        {
            _pdfPage = pdfPage;
        }

        public override Task Execute()
        {
            
        }

        public override Task Reject()
        {
            
        }
    }
}