using HomeTask4.Core.Entities;
namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class MeasureRepository : Repository<Measure>
    {
        public MeasureRepository(Task4DBContext context) : base(context) { }
    }
}