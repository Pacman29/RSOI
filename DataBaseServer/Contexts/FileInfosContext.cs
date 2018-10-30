using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Annotations;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FileInfo>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<FileInfo>()
                .HasIndex(e => e.Md5)
                .IsUnique(true);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.Md5)
                .IsRequired(true)
                .HasMaxLength(32);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.changed);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.fileLength)
                .IsRequired(true);
            
            modelBuilder.Entity<FileInfo>()
                .Property(e => e.Version);
            
            modelBuilder.Entity<FileInfo>()
                .Property(e => e.Path)
                .IsRequired(true)
                .HasMaxLength(250);
            
        }
    }
}