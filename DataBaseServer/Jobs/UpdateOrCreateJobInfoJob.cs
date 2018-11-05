using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using DataBaseServer.DBO;
using GRPCService.GRPCProto;
using JobExecutor;

namespace DataBaseServer.Jobs
{
    public class UpdateOrCreateJobInfoJob : BaseJob
    {
        private readonly JobInfo _jobInfo;
        private readonly JobsDbManager _jobsDbManager;


        public UpdateOrCreateJobInfoJob(JobsDbManager jobsDbManager, JobInfo jobInfo)
        {
            _jobsDbManager = jobsDbManager;
            _jobInfo = jobInfo;
        }

        private async Task<Job> CreateJob()
        {
            return await _jobsDbManager.AddAsync(new Job()
            {
                GUID = _jobInfo.JobId,
                status = _jobInfo.JobStatus
            });
        }

        private async Task<Job> UpdateJob()
        {
            return await _jobsDbManager.UpdateJobStatus(_jobInfo.JobId, _jobInfo.JobStatus);
        }
        
        public override async Task ExecuteAsync()
        {
            Job job = null;
            if (this._jobInfo.JobId.Length == 0)
            {
                _jobInfo.JobId = System.Guid.NewGuid().ToString();
                job = await CreateJob();
            }
            else
            {
                var result = _jobsDbManager.FindByGuid(this._jobInfo.JobId);
                if (result == null)
                    job = await CreateJob();
                else
                    job = await UpdateJob();
            }
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, job.GUID);
                this.Bytes = ms.ToArray();
            }
        }
    }
}