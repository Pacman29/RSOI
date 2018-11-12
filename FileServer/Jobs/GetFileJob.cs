using System.IO;
using System.Threading.Tasks;
using JobExecutor;

namespace FileServer.Jobs
{
    public class GetFileJob : BaseJob
    {
        private string _path;

        public GetFileJob(string path)
        {
            _path = path;
        }

        public override async Task ExecuteAsync()
        {
            using (var fileStream = new FileStream(_path,FileMode.Open))
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                this.Bytes = memoryStream.ToArray();
            }
        }

        public override async Task Reject()
        {
            
        }
    }
}