using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using System.Threading.Tasks;

namespace HomeTask4.Infrastructure.Data
{
    public interface IUnitOfWork
    {
        IRepository<Ingredient> Ingredients { get; }
        IRepository<Category> Categories { get; }
        IRepository<Recipe> Recipes { get; }
        IRepository<RecipeStep> RecipeSteps { get; }
        IRepository<Measure> Measures { get; }
        IRepository<IngredientDetail> IngredientDetails { get; }
        Task SaveChangesAsync();
    }
}
