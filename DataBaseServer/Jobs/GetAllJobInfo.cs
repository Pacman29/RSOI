using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using JobExecutor;
using Models.Responses;

namespace DataBaseServer.Jobs
{
    public class GetAllJobInfo : BaseJob
    {
        private readonly JobsDbManager _jobsDbManager;

        public GetAllJobInfo(JobsDbManager jobsDbManager)
        {
            _jobsDbManager = jobsDbManager;
        }

        public override async Task ExecuteAsync()
        {
            var jobs = await _jobsDbManager.GetAllAsync();
            var jobInfos = new List<JobInfoBase>();

            foreach (var job in jobs)
            {
                jobInfos.Add(new JobInfoBase()
                {
                    JobId = job.GUID,
                    JobStatus = job.status.ToString()
                });
            }
            
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, jobInfos);
                this.Bytes = ms.ToArray();
            }
        }
    }
}