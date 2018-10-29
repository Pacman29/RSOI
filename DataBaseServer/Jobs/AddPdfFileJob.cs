using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using DataBaseServer.Exceptions.DBExceptions;
using JobExecutor;
using FileInfo = DataBaseServer.DBO.FileInfo;

namespace DataBaseServer.Jobs
{
    public class AddPdfFileJob : BaseJob
    {
        private readonly FileInfosContext _fileInfosContext;
        private readonly FileInfo _fileInfo;

        public AddPdfFileJob(FileInfosContext fileInfosContext, FileInfo fileInfo)
        {
            _fileInfosContext = fileInfosContext;
            _fileInfo = fileInfo;
        }

        public override async Task ExecuteAsync()
        {
            var result = await _fileInfosContext.AddAsync(_fileInfo);
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
            var result = await _fileInfosContext.DeleteAsync(_fileInfo);
            if (!result)
            {
                throw new DeleteException();
            }
        }
    }
}