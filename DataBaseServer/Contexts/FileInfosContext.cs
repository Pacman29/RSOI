using System.Collections.Generic;
using System.Threading.Tasks;
using DataBaseServer.DBO;
using Microsoft.EntityFrameworkCore;

namespace DataBaseServer.Contexts
{
    public class FileInfosContext : DbContext, IContext<FileInfo>
    {
        private BaseContext<FileInfo> baseContext;
        private DbSet<FileInfo> FileInfos { get; set; }
        
        public FileInfosContext()
        {
            baseContext = new BaseContext<FileInfo>(this, FileInfos);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DataBaseConnection.GetDatabaseConnection(optionsBuilder);
        }

        public Task<List<FileInfo>> GetAllAsync()
        {
            return baseContext.GetAllAsync();
        }

        public Task<bool> AddAsync(FileInfo source)
        {
            return baseContext.AddAsync(source);
        }

        public Task<FileInfo> FindByIdAsync(int id)
        {
            return baseContext.FindByIdAsync(id);
        }

        public Task<bool> UpdateAsync(FileInfo source)
        {
            return baseContext.UpdateAsync(source);
        }

        public Task<bool> DeleteAsync(FileInfo source)
        {
            return baseContext.DeleteAsync(source);
        }
    }
}