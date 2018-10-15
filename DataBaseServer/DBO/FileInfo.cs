using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GRPCService.DataBaseProto;

namespace DataBaseServer.DBO
{
    [Table("FileInfos")]
    public class FileInfo : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public DateTime changed { get; set; }

        [Index(IsUnique=true)]
        public string Md5 { get; set; }
        public long fileLength { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }

        public FileInfo()
        {
            changed = DateTime.Now;
        }
        
        public FileInfo(string md5, long fileLength) : this()
        {
            Md5 = md5;
            this.fileLength = fileLength;
        }

        public static FileInfo fromPdfFileInfo(PdfFileInfo pdfFileInfo)
        {
            return new FileInfo(pdfFileInfo.MD5,pdfFileInfo.FileLength);
        }

        
    }
}