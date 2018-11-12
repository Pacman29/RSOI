using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using DataBaseServer.Exceptions.DBExceptions;
using JobExecutor;
using FileInfo = DataBaseServer.DBO.FileInfo;

namespace DataBaseServer.Jobs
{
    public class AddFileInfoJob : BaseJob
    {
        private readonly FileInfosDbManager _fileInfosDbManager;
        private readonly FileInfo _fileInfo;

        public AddFileInfoJob(FileInfosDbManager fileInfosDbManager, FileInfo fileInfo)
        {
            _fileInfosDbManager = fileInfosDbManager;
            _fileInfo = fileInfo;
        }

        public override async Task ExecuteAsync()
        {
            var result = await _fileInfosDbManager.AddAsync(_fileInfo);
            if (result == null)
                throw new AddException();
            
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, result.Id);
                this.Bytes = ms.ToArray();
            }
        }

        public override async Task Reject()
        {
            var result = await _fileInfosDbManager.DeleteAsync(_fileInfo);
            if (!result)
            {
                throw new DeleteException();
            }
        }
    }
}