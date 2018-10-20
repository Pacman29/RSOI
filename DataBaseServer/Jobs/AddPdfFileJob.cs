using System.Threading.Tasks;
using DataBaseServer.Contexts;
using DataBaseServer.DBO;
using DataBaseServer.Exceptions.DBExceptions;
using JobExecutor;

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

        public override async Task Execute()
        {
            var result = await _fileInfosContext.AddAsync(_fileInfo);
            if (!result)
            {
                throw new AddException();
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