using System;
using System.Collections.Generic;
using GRPCService.GRPCProto;

namespace DataBaseServer.DBO
{
    public class Job : IEntity
    {
        
        public DateTime changed { get; set; }
        public string GUID { get; set; }
        public EnumJobStatus status { get; set; }
        public List<FileInfo> fileInfos { get; set; }
        
        
        public Job()
        {
            this.changed = DateTime.Now;
        }
    }
}