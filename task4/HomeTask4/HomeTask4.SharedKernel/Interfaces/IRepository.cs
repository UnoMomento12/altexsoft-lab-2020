using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTask4.SharedKernel.Interfaces
{
    public interface IRepository {
        Task<T> GetByIdAsync<T>(int id) where T : BaseEntity;
        Task<List<T>> ListAsync<T>() where T : BaseEntity;
        Task AddAsync<T>(T entity) where T : BaseEntity;
        Task UpdateAsync<T>(T entity) where T : BaseEntity;
        Task DeleteAsync<T>(T entity) where T : BaseEntity;
    }
    //public interface IRepository<T> : IRepository where T : BaseEntity
    //{
    //    Task<T> GetByIdAsync(int id);
    //    Task<List<T>> GetAllAsync();
    //    Task AddAsync(T entity);
    //    Task UpdateAsync(T entity);
    //    Task DeleteAsync(T entity);
    //}
}
