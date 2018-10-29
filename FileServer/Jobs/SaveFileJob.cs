using System.IO;
using System.Threading.Tasks;
using JobExecutor;

namespace FileServer.Jobs
{
    public class SaveFileJob : BaseJob
    {
        private readonly MemoryStream _stream;
        private readonly string _path;

        public SaveFileJob(string path, MemoryStream stream)
        {
            _path = path;
            _stream = stream;
        }

        public override async Task ExecuteAsync()
        {
            using (var fileStream = new FileStream(_path, FileMode.OpenOrCreate))
            {
                _stream.WriteTo(fileStream);
            }
        }

        public override async Task Reject()
        {
            
        }
    }
}