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
        public long FileLength { get; set; }
        public string Path { get; set; }
        public EnumFileType FileType { get; set; }
        public string JobGuidFk { get; set; }
        public Job Job { get; set; }
        public long PageNo { get; set; } 

        public FileInfo()
        {
            changed = DateTime.Now;
        }

        public static FileInfo FromPdfFileInfo(GRPCService.GRPCProto.FileInfo fileInfo)
        {
            return new FileInfo()
            {
                Md5 = fileInfo.MD5,
                FileLength = fileInfo.FileLength,
                Path = fileInfo.Path,
                JobGuidFk = fileInfo.JobId,
                FileType = fileInfo.FileType
            };
        }

        
    }
}