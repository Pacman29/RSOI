using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using JobExecutor;
using FileInfo = DataBaseServer.DBO.FileInfo;

namespace DataBaseServer.Jobs
{
    public class DeleteFileInfo : BaseJob
    {
        private readonly FileInfosDbManager _fileInfosDbManager;
        private readonly FileInfo _fileInfo;

        public DeleteFileInfo(FileInfosDbManager fileInfosDbManager, FileInfo fileInfo)
        {
            _fileInfosDbManager = fileInfosDbManager;
            _fileInfo = fileInfo;
        }

        public override async Task ExecuteAsync()
        {
            Console.Write("delete id: "+_fileInfo.Id);
            var fileInfo = await _fileInfosDbManager.FindByIdAsync(_fileInfo.Id);
            var res = await _fileInfosDbManager.DeleteAsync(fileInfo);
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, res);
                this.Bytes = ms.ToArray();
            }
        }
    }
}