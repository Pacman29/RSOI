using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace RSOI.Services
{
    public interface IDataBaseService : IDisposable
    {
        Task CreatePdfFile(PdfFileInfo pdfFileInfo);
        Task DoneJob(Guid jobId);
    }
}