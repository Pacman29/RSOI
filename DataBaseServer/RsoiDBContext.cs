
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataBaseServer.DBO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace DataBaseServer
{
    public class RsoiDBContext : DbContext
    {

        private Microsoft.EntityFrameworkCore.DbSet<FileInfo> FileInfos { get; set; }

        public async Task<EntityEntry> SetFileInfo(FileInfo fileInfo)
        {
            EntityEntry result = null;
            try
            {
                result = await FileInfos.AddAsync(fileInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }
        
        public async Task<List<FileInfo>> GetAllFileInfos()
        {
            return await FileInfos.ToListAsync();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=rsoi_database;Username=rsoi;Password=rsoi");
        }
    }
}