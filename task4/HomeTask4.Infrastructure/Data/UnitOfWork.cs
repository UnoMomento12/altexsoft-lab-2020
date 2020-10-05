using System.Threading.Tasks;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;

namespace HomeTask4.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private  Task4DBContext _context;

        public IRepository<Ingredient> Ingredients { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Recipe> Recipes { get; }
        public IRepository<RecipeStep> RecipeSteps { get; }
        public IRepository<Measure> Measures { get; }
        public IRepository<IngredientDetail> IngredientDetails { get; }

        public UnitOfWork(Task4DBContext context, IRepository<Category> repositoryC, IRepository<Recipe> repositoryR,
           IRepository<RecipeStep> repositoryRS, IRepository<Ingredient> repositoryIng, IRepository<IngredientDetail> repositoryIngD, IRepository<Measure> repositoryM )
        {
            _context = context;
            Ingredients= repositoryIng;
            Categories = repositoryC;
            Recipes = repositoryR;
            RecipeSteps = repositoryRS;
            Measures = repositoryM;
            IngredientDetails = repositoryIngD;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
