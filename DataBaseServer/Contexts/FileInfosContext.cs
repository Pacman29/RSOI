using System.Collections.Generic;
using System.Threading.Tasks;
using DataBaseServer.DBO;
using Microsoft.EntityFrameworkCore;

namespace DataBaseServer.Contexts
{
    public class FileInfosContext : DbContext, IContext<FileInfo>
    {
        private readonly BaseContext<FileInfo> _baseContext;
        private DbSet<FileInfo> FileInfos { get; set; }
        
        public FileInfosContext()
        {
            _baseContext = new BaseContext<FileInfo>(this, FileInfos);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DataBaseConnection.GetDatabaseConnection(optionsBuilder);
        }

        public Task<List<FileInfo>> GetAllAsync()
        {
            return _baseContext.GetAllAsync();
        }

        public Task<FileInfo> AddAsync(FileInfo source)
        {
            return _baseContext.AddAsync(source);
        }

        public Task<FileInfo> FindByIdAsync(int id)
        {
            return _baseContext.FindByIdAsync(id);
        }

        public Task<bool> UpdateAsync(FileInfo source)
        {
            return _baseContext.UpdateAsync(source);
        }

        public Task<bool> DeleteAsync(FileInfo source)
        {
            return _baseContext.DeleteAsync(source);
        }
    }
}