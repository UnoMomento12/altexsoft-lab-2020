using task2.Repositories;
using task2.Models;
namespace task2.UnitsOfWork
{
    interface IUnitOfWork
    {
        public IRepository<Recipe> Recipes { get; }
        public IRepository<Ingredient> Ingredients { get; }
        public IRepository<Category> Categories { get; }
        public void Save();

    }
}
