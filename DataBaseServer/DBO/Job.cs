using System;
using System.Collections.Generic;

namespace DataBaseServer.DBO
{
    public class Job : IEntity
    {
        public enum JobStatusEnum
        {
            Done, Execute, Error, Reject
        }
        
        public DateTime changed { get; set; }
        public string GUID { get; set; }
        public JobStatusEnum status { get; set; }
        public List<FileInfo> fileInfos { get; set; }
        
        
        public Job()
        {
            this.changed = DateTime.Now;
        }
    }
}