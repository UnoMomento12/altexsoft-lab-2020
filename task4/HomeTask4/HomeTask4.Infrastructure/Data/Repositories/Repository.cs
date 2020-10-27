using HomeTask4.SharedKernel;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class Repository : IRepository 
    {
        private Task4DBContext _context;
        public Repository(Task4DBContext context)
        {
            _context = context;
        }

        public async Task AddAsync<T>(T entity) where T : BaseEntity
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync<T>(T entity) where T : BaseEntity
        {
            _context.Set<T>().Remove(entity);
            return _context.SaveChangesAsync();
        }

        public async Task<List<T>> ListAsync<T>() where T : BaseEntity
        {
            return await _context.Set<T>().ToListAsync();
        }
        
        public async Task<T> GetByIdAsync<T>(int id) where T : BaseEntity
        {
            return await _context.Set<T>().FindAsync(id).AsTask();
        }

        public Task UpdateAsync<T>(T entity) where T : BaseEntity
        {
            _context.Set<T>().Update(entity);
            return _context.SaveChangesAsync();
        }

        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            return _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public Task<List<T>> WhereAsync<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            return _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task AddRangeAsync<T>(List<T> entities) where T : BaseEntity
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
