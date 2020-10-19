using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HomeTask4.SharedKernel.Interfaces
{
    public interface IRepository {
        Task<T> GetByIdAsync<T>(int id) where T : BaseEntity;
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T:BaseEntity;
        Task<List<T>> WhereAsync<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;
        Task<List<T>> ListAsync<T>() where T : BaseEntity;
        Task AddAsync<T>(T entity) where T : BaseEntity;
        Task UpdateAsync<T>(T entity) where T : BaseEntity;
        Task DeleteAsync<T>(T entity) where T : BaseEntity;
        Task AddRangeAsync<T>(List<T> entities) where T : BaseEntity;
    }
}
