using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using JobExecutor;

namespace FileServer.Jobs
{
    public class DeleteFile : BaseJob
    {
        private readonly string _path;

        public DeleteFile(string path)
        {
            _path = path;
        }

        public override async Task ExecuteAsync()
        {
            var res = false;
            if (File.Exists(this._path))
            {
                File.Delete(this._path);
                res = true;
            }
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, res);
                this.Bytes = ms.ToArray();
            } 
        }
    }
}