using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using FileInfo = GRPCService.GRPCProto.FileInfo;

namespace Models.Requests
{
    public class PdfFile
    {
        private object threadLock = new object();
        public long Dpi { get; set; } = 300;
        public long PageNo { get; set; } = 0;
        public IFormFile File { get; set; }

        public async Task<FileInfo> GetFileInfo()
        {
            var sBuilder = new StringBuilder();
            using (var md5Hash = MD5.Create())
            {
                var hash = md5Hash.ComputeHash(await ReadFile());
                foreach (var data in hash)
                    sBuilder.Append(data.ToString("x2"));
            } 
           return new FileInfo
           {
               FileLength = File.Length, 
               MD5 = sBuilder.ToString(),
               FileType = EnumFileType.Pdf
           };
        }
        
        public async Task<byte[]> ReadFile()
        {
            lock (threadLock)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    this.File.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}