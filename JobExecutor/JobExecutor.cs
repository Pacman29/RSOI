using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace JobExecutor
{
    public  sealed class JobExecutor : IJobExecutor
    {
        private readonly object balanceLock = new object();
        private ConcurrentDictionary<Guid,BaseJob> jobs = new ConcurrentDictionary<Guid, BaseJob>();

        private JobExecutor()
        {
        }
        
        public static JobExecutor Instance => Nested.instance;

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly JobExecutor instance = new JobExecutor();
        }

        public void AddJob(BaseJob job,Action<Guid> onOk,Action<Guid,Exception> onError)
        {
            if (job.Guid == null)
                throw  new Exception("Job uid is null");

            var guid = (Guid) job.Guid;
            try
            {
                var result = jobs.TryAdd(guid,job);
                if (!result)
                    onError(guid,new Exception("job not added"));
                Task.Run(async () =>
                {
                    try
                    {
                        await job.Execute();
                        onOk(guid);
                    }
                    catch(Exception e)
                    {
                        onError(guid,e);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public void AddJob(BaseJob job)
        {
            if (job.Guid == null)
                throw  new Exception("Job uid is null");

            var guid = (Guid) job.Guid;
            try
            {
                var result = jobs.TryAdd(guid,job);
                if (!result)
                    throw new Exception("job not added");
                Task.Run(async () =>
                {
                    await job.Execute();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public BaseJob GetJob(Guid id)
        {
            lock (balanceLock)
            {
                if (!jobs.TryGetValue(id, out var result))
                {
                    throw new Exception("job not found");
                }

                return result;
            }
        }
        
        public BaseJob DeleteJob(Guid id)
        {
            lock (balanceLock)
            {
                if (!jobs.Remove(id, out var result))
                {
                    throw new Exception("job not deleted");
                }

                return result;
            }
        }

        public async Task<BaseJob> RejectJobAsync(Guid id)
        {
            var job = this.DeleteJob(id);
            await job.Reject();
            job.JobStatus = EnumJobStatus.Rejected;
            return job;
        }
        
        public async Task<BaseJob> DoneJobAsync(Guid id)
        {
            var job = this.DeleteJob(id);
            job.JobStatus = EnumJobStatus.Done;
            return job;
        }

        public void SetJobStatus(Guid id, EnumJobStatus status)
        {
            lock (balanceLock)
            {
                var job = this.GetJob(id);
                job.JobStatus = status;
            }
        }
    }
}