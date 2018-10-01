using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GRPCService.DataBaseProto;
using Microsoft.AspNetCore.Http;

namespace Models.Requests
{
    public class PdfFile
    {
        public long Dpi { get; set; } = 300;
        public long PageNo { get; set; } = 0;
        public IFormFile File { get; set; }

        public async Task<PdfFileInfo> GetPdfFileInfo()
        {
            var sBuilder = new StringBuilder();
            using (var md5Hash = MD5.Create())
            {
                var hash = md5Hash.ComputeHash(await ReadFile());
                foreach (var data in hash)
                    sBuilder.Append(data.ToString("x2"));
            } 
           return new PdfFileInfo {FileLength = File.Length, MD5 = sBuilder.ToString()};
        }
        
        public async Task<byte[]> ReadFile()
        {
            var s = this.File.OpenReadStream();
            var buffer = new byte[s.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = await s.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await ms.WriteAsync(buffer, 0, read);
                }
                return ms.ToArray();
            }
        } 
    }
}