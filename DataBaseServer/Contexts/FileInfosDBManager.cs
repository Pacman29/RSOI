using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Annotations;
using System.Threading.Tasks;
using DataBaseManager;
using DataBaseServer.DBO;
using Microsoft.EntityFrameworkCore;

namespace DataBaseServer.Contexts
{
    public class FileInfosDbManager : IDbManager<FileInfo>
    {
        private readonly CommonDbManager<FileInfo> _commonDbManager;
        private DbSet<FileInfo> FileInfos { get; set; }
        
        public FileInfosDbManager(BaseContext context)
        {
            FileInfos = context.FileInfos;
            _commonDbManager = new CommonDbManager<FileInfo>(context, FileInfos);
        }

        public Task<List<FileInfo>> GetAllAsync()
        {
            return _commonDbManager.GetAllAsync();
        }

        public Task<FileInfo> AddAsync(FileInfo source)
        {
            return _commonDbManager.AddAsync(source);
        }

        public Task<FileInfo> FindByIdAsync(int id)
        {
            return _commonDbManager.FindByIdAsync(id);
        }

        public Task<FileInfo> UpdateAsync(FileInfo source)
        {
            return _commonDbManager.UpdateAsync(source);
        }

        public Task<bool> DeleteAsync(FileInfo source)
        {
            return _commonDbManager.DeleteAsync(source);
        }
    }
}