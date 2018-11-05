using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataBaseServer.DBO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataBaseServer.Contexts
{
    public class CommonDbManager<T>: IDbManager<T> where T : class, IEntity 
    {
        private DbSet<T> _dbSet { get; set; }
        private DbContext _context;

        public CommonDbManager(DbContext context, DbSet<T> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        
        public async Task<T> AddAsync(T source)
        {
            T result = null;
            var state = await _dbSet.AddAsync(source);
            if (state.State == EntityState.Added)
            {
                await _context.SaveChangesAsync();
                result = state.Entity;
            }

            return result;
        }
        
        public async Task<T> FindByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(T source)
        {
            bool result;
            source.changed = DateTime.Now;
            var state = _dbSet.Update(source);
            if (state.State == EntityState.Modified)
            {
                await _context.SaveChangesAsync();
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> DeleteAsync(T source)
        {
            bool result;
            var state = _dbSet.Remove(source);
            if (state.State == EntityState.Deleted)
            {
                await _context.SaveChangesAsync();
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
       
    }
}