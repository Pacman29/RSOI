using System;
using System.Threading.Tasks;
using Google.Protobuf;
using GRPCService.GRPCProto;

namespace JobExecutor
{
    public class BaseJob
    {
        public Guid? Guid { get; set; } = null;
        public BaseJob RootJob { get; set; }
        public IJobExecutor Executor { get; set; }
        public byte[] Bytes { get; set; }
        
        
        private EnumJobStatus _jobStatus;
        public EnumJobStatus JobStatus
        {
            get => _jobStatus;
            set
            {
                switch (value)
                {
                    case EnumJobStatus.Done:
                        OnDone();
                        break;
                    case EnumJobStatus.Error:
                        OnError();
                        break;
                    case EnumJobStatus.Rejected:
                        OnReject();
                        break;
                    case EnumJobStatus.Execute:
                        OnExecute();
                        break;
                }
                this._jobStatus = value;
            }
        }

        public virtual void OnDone()
        {
            
        }

        public virtual void OnError()
        {
            
        }
        public virtual void OnReject() 
        {
            
        }
        public virtual void OnExecute()
        {
            
        }
        
        public virtual async Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }

        public virtual async Task Reject()
        {
            throw new NotImplementedException();
        }

        public JobInfo GetJobInfo()
        {
            var guid = this.Guid;
            if (guid != null)
                return new JobInfo()
                {
                    JobId = ((Guid) guid).ToString(),
                    JobStatus = this.JobStatus,
                };
            return null;
        }
        
        public JobInfoWithBytes GetJobInfoWithBytes()
        {
            var guid = this.Guid;
            if (guid != null)
                return new JobInfoWithBytes()
                {
                    Bytes = ByteString.CopyFrom(Bytes),
                    JobInfo = GetJobInfo()
                };
            return null;
        }
        
    }
}