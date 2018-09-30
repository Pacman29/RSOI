using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Models.Requests
{
    public class PdfFile
    {
        public long Dpi { get; set; } = 300;
        public long PageNo { get; set; } = 0;
        public IFormFile File { get; set; }
    }
}