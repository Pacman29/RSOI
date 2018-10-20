using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobExecutor
{
    public  sealed class JobExecutor : IJobExecutor
    {
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
            try
            {
                var result = jobs.TryAdd(job.GetJobId(),job);
                if (!result)
                    onError(job.GetJobId(),new Exception("job not added"));
                Task.Run(async () =>
                {
                    try
                    {
                        await job.Execute();
                        onOk(job.GetJobId());
                    }
                    catch(Exception e)
                    {
                        onError(job.GetJobId(),e);
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
            try
            {
                var result = jobs.TryAdd(job.GetJobId(),job);
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
            if (!jobs.TryGetValue(id, out var result))
            {
                throw new Exception("job not found");
            }
            return result;
        }
        
        public BaseJob DeleteJob(Guid id)
        {
            if (!jobs.Remove(id, out var result))
            {
                throw new Exception("job not deleted");
            }
            return result;
        }
    }
}