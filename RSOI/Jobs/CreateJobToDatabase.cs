using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class CreateJobToDatabase : GateWayJob<string>
    {
        public IDataBaseService DataBaseService { get; set; }

        public CreateJobToDatabase()
        {
            this.OnDone += async job =>
            {
                this.InvokeOnHaveResult(BytesDeserializer<string>.Deserialize(job));
            };
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.UpdateOrCreateJob(new JobInfo());
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}