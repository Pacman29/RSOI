using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GRPCService.GRPCProto;

namespace DataBaseServer.DBO
{
    public class FileInfo : IEntity
    {
        public int Id { get; set; }
        public string Md5 { get; set; }
        public DateTime changed { get; set; }
        public long fileLength { get; set; }
        public DateTime Version { get; set; }
        public string Path { get; set; }
        public string JobGuidFk { get; set; }
        public Job Job { get; set; }

        public FileInfo()
        {
            changed = DateTime.Now;
        }

        public static FileInfo FromPdfFileInfo(PdfFileInfo pdfFileInfo)
        {
            return new FileInfo()
            {
                Md5 = pdfFileInfo.MD5,
                fileLength = pdfFileInfo.FileLength,
                Path = pdfFileInfo.Path,
                JobGuidFk = pdfFileInfo.JobId
            };
        }

        
    }
}