using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using JobExecutor;

namespace DataBaseServer.Jobs
{
    public class DeleteJobInfoJob : BaseJob
    {
        private readonly JobsDbManager _jobsDbManager;
        private readonly string _jobId;
        

        public DeleteJobInfoJob(JobsDbManager jobsDbManager,string jobId)
        {
            _jobsDbManager = jobsDbManager;
            _jobId = jobId;
        }

        public override async Task ExecuteAsync()
        {
            var result = false;
            var job = await this._jobsDbManager.FindByGuid(this._jobId);
            if (job != null)
                result = await this._jobsDbManager.DeleteAsync(job);
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, result);
                this.Bytes = ms.ToArray();
            }
        }
    }
}