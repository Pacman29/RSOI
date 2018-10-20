using System;
using System.Threading.Tasks;

namespace JobExecutor
{
    public class BaseJob
    {
        private readonly Guid guid= System.Guid.NewGuid();
        public virtual async Task Execute()
        {
            throw new NotImplementedException();
        }
        public virtual async Task Reject()
        {
            throw new NotImplementedException();
        }

        public Guid GetJobId()
        {
            return guid;
        }
    }
}