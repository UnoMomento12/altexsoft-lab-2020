using System.Threading.Tasks;

namespace HomeTask4.SharedKernel.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository Repository { get; }
        Task SaveChangesAsync();
    }
}
