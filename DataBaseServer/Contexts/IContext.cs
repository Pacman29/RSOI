using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataBaseServer.DBO;

namespace DataBaseServer.Contexts
{
    public interface IContext<T> where T : class, IEntity 
    {
        Task<List<T>> GetAllAsync();

        Task<T> AddAsync(T source);

        Task<T> FindByIdAsync(int id);

        Task<bool> UpdateAsync(T source);

        Task<bool> DeleteAsync(T source);
    }
}