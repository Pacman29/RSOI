using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GRPCService.GRPCProto;

namespace DataBaseServer.DBO
{
    [Table("FileInfos")]
    public class FileInfo : IEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime changed { get; set; }

        [Required]
        [StringLength(32)]
        [Index(IsUnique = true)]
        public string Md5 { get; set; }
        [Required]
        public long fileLength { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
        
        [StringLength(250)]
        public string Path { get; set; }

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
                Path = pdfFileInfo.Path
            };
        }

        
    }
}