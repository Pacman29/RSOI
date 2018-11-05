using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace JobExecutor
{
    public  sealed class JobExecutor : IJobExecutor
    {
        private readonly object _balanceLock = new object();
        private readonly ConcurrentDictionary<Guid,BaseJob> _jobs = new ConcurrentDictionary<Guid, BaseJob>();
        private struct jobPack
        {
            public BaseJob Job;
            public Action<Guid> OnOk;
            public Action<Guid, Exception> OnError;
        }
        private readonly ConcurrentQueue<jobPack> _synchroniousJobs = new ConcurrentQueue<jobPack>();
        private bool IsSynchroniousJobWork { get; set;}
        
        private JobExecutor()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    while (!_synchroniousJobs.IsEmpty)
                    {
                        IsSynchroniousJobWork = true;
                        if (_synchroniousJobs.TryDequeue(out var jobPack) && jobPack.Job.Guid != null)
                        {
                            var guid = (Guid) jobPack.Job.Guid;
                            try
                            {
                                await jobPack.Job.ExecuteAsync();
                                jobPack.OnOk?.Invoke(guid);
                            }
                            catch (Exception e)
                            {
                                jobPack.OnError?.Invoke(guid, e);
                            }
                        }
                    }
                    IsSynchroniousJobWork = false;
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            });
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
        
        //TODO: rewrite to one method
        public void JobAsyncExecute(BaseJob job,Action<Guid> onOk,Action<Guid,Exception> onError)
        {
            if (job.Guid == null)
                throw  new Exception("Job uid is null");

            job.Executor = this;
            var guid = (Guid) job.Guid;
            try
            {
                var result = _jobs.TryAdd(guid,job);
                if (!result)
                    onError(guid,new Exception("job not added"));
                Task.Run(async () =>
                {
                    try
                    {
                        await job.ExecuteAsync();
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

        public void JobAsyncExecute(BaseJob job)
        {
            if (job.Guid == null)
                throw  new Exception("Job uid is null");

            job.Executor = this;
            var guid = (Guid) job.Guid;
            try
            {
                var result = _jobs.TryAdd(guid,job);
                if (!result)
                    throw new Exception("job not added");
                Task.Run(async () =>
                {
                    await job.ExecuteAsync();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public void JobExecute(BaseJob job, Action<Guid> onOk, Action<Guid, Exception> onError)
        {
            if (job.Guid == null)
                throw  new Exception("Job uid is null");
            var guid = (Guid) job.Guid;
            job.Executor = this;
            _synchroniousJobs.Enqueue(new jobPack()
            {
                Job = job,
                OnError = onError,
                OnOk = onOk
            });
            var result = _jobs.TryAdd(guid,job);
            if (!result)
                onError(guid,new Exception("job not added"));
        }

        public void JobExecute(BaseJob job)
        {
            if (job.Guid == null)
                throw  new Exception("Job uid is null");
            var guid = (Guid) job.Guid;
            job.Executor = this;
            _synchroniousJobs.Enqueue(new jobPack()
            {
                Job = job,
            });
            _jobs.TryAdd(guid,job);
        }

        public BaseJob GetJob(Guid id)
        {
            lock (_balanceLock)
            {
                if (!_jobs.TryGetValue(id, out var result))
                {
                    throw new Exception("job not found");
                }

                return result;
            }
        }
        
        public BaseJob DeleteJob(Guid id)
        {
            lock (_balanceLock)
            {
                if (!_jobs.Remove(id, out var result))
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

        public void SetJobStatus(Guid id, EnumJobStatus status, byte[] bytes)
        {
            lock (_balanceLock)
            {
                var job = this.GetJob(id);
                job.Bytes = bytes;
                job.JobStatus = status;
            }
        }

        public void SetJobStatus(Guid id, EnumJobStatus status)
        {
            lock (_balanceLock)
            {
                var job = this.GetJob(id);
                job.JobStatus = status;
            }
        }

        public void SetJobStatusByServiceGuid(Guid id, EnumJobStatus status)
        {
            lock (_balanceLock)
            {
                var job = _jobs.FirstOrDefault(pair => pair.Value.ServiceGuid == id);
                job.Value.JobStatus = status;
            }
        }
        
        public void SetJobStatusByServiceGuid(Guid id, EnumJobStatus status, byte[] bytes)
        {
            lock (_balanceLock)
            {
                var job = _jobs.FirstOrDefault(pair => pair.Value.ServiceGuid == id);
                job.Value.Bytes = bytes;
                job.Value.JobStatus = status;
            }
        }
    }
}