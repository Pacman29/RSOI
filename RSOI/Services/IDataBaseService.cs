using System;
using GRPCService.DataBaseProto;

namespace RSOI.Services
{
    public interface IDataBaseService : IDisposable
    {
        void CreatePdfFile(PdfFileInfo pdfFileInfo);
    }
}