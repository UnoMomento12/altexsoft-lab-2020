using System.Threading.Tasks;
using HomeTask4.SharedKernel.Interfaces;

namespace HomeTask4.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private  Task4DBContext _context;

        public IRepository Repository { get; }

        public UnitOfWork(Task4DBContext context, IRepository repository )
        {
            _context = context;
            Repository = repository;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
