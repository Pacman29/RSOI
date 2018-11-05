using System.Threading.Tasks;
using GRPCService.GRPCProto;
using Microsoft.AspNetCore.Mvc;
using PdfFile = GRPCService.GRPCProto.PdfFile;

namespace RSOI.Services
{
    public interface IRecognizeService
    {
        Task<JobInfo> RecognizePdf(PdfFile pdfFile);
    }
}