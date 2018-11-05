using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBaseServer.DBO;
using Microsoft.EntityFrameworkCore;

namespace DataBaseServer.Contexts
{
    public class JobsDbManager : IDbManager<Job>
    {
        private readonly CommonDbManager<Job> _commonDbManager;
        private DbSet<Job> Jobs { get; set; }

        public JobsDbManager(BaseContext context)
        {
            Jobs = context.Jobs;
            _commonDbManager = new CommonDbManager<Job>(context, Jobs);
        }

        public Task<List<Job>> GetAllAsync()
        {
            return _commonDbManager.GetAllAsync();
        }

        public Task<Job> AddAsync(Job source)
        {
            return _commonDbManager.AddAsync(source);
        }

        public Task<Job> FindByIdAsync(int id)
        {
            return _commonDbManager.FindByIdAsync(id);
        }

        public Task<bool> UpdateAsync(Job source)
        {
            return _commonDbManager.UpdateAsync(source);
        }

        public Task<bool> DeleteAsync(Job source)
        {
            return _commonDbManager.DeleteAsync(source);
        }

        public async Task<Job> FindByGUID(string guid)
        {
            return await Jobs.Where(job => job.GUID == guid).FirstAsync();
        }

        public async Task<bool> UpdateJobStatus(string guid, Job.JobStatusEnum status)
        {
            var _job = await Jobs.Where(job => job.GUID == guid).FirstAsync();
            return await _commonDbManager.UpdateAsync(_job);
        }
    }
}