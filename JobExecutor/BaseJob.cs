using System;
using System.Threading.Tasks;
using Google.Protobuf;
using GRPCService.GRPCProto;

namespace JobExecutor
{
    public class BaseJob
    {
        public Guid? Guid { get; set; } = null;
        public Guid? ServiceGuid { get; set; } = null;
        public BaseJob RootJob { get; set; }
        public IJobExecutor Executor { get; set; }
        public byte[] Bytes { get; set; }

        //public Action<BaseJob> OnDone = null;
        //public Action<BaseJob> OnError = null;
        //public Action<BaseJob> OnReject = null;
        //public Action<BaseJob> OnExecute = null;

        public delegate void JobEvent(BaseJob job);

        public event JobEvent OnDone;
        public event JobEvent OnError;
        public event JobEvent OnReject;
        public event JobEvent OnExecute;
        
        private EnumJobStatus _jobStatus;
        public EnumJobStatus JobStatus
        {
            get => _jobStatus;
            set
            {
                switch (value)
                {
                    case EnumJobStatus.Done:
                        OnDone?.Invoke(this);
                        break;
                    case EnumJobStatus.Error:
                        OnError?.Invoke(this);
                        break;
                    case EnumJobStatus.Rejected:
                        OnReject?.Invoke(this);
                        break;
                    case EnumJobStatus.Execute:
                        OnExecute?.Invoke(this);
                        break;
                }
                this._jobStatus = value;
            }
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