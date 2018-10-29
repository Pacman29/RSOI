using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PdfFile = GRPCService.GRPCProto.PdfFile;

namespace RSOI.Services
{
    public interface IRecognizeService
    {
        Task RecognizePdf(PdfFile pdfFile);
    }
}