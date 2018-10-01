using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataBaseServer.DBO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataBaseServer.Contexts
{
    public class BaseContext<T> : DbContext where T : class, IEntity
    {
        private DbSet<T> _dbSet;
        
        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        
        public async Task AddAsync(T source)
        {
            var result = await _dbSet.AddAsync(source);
            if(result.State == EntityState.Added)
                await SaveChangesAsync();
            else
            {
                throw 
            }
        }
        
        public async Task<T> FindByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async UpdateAsync(T source)
        {
            source.changed = DateTime.Now;
            return await _dbSet.UpdateAsync(source);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=rsoi_database;Username=rsoi;Password=rsoi");
        }
    }
}