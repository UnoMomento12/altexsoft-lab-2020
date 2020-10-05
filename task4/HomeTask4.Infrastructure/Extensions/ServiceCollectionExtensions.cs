using HomeTask4.Core.Entities;
using HomeTask4.Infrastructure.Data;
using HomeTask4.Infrastructure.Data.Repositories;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HomeTask4.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<Task4DBContext>(opts => opts.UseSqlServer(connectionString));
            services.AddScoped<IRepository<Category>, CategoryRepository>();
            services.AddScoped<IRepository<Recipe>, RecipeRepository>();
            services.AddScoped<IRepository<RecipeStep>, RecipeStepRepository>();
            services.AddScoped<IRepository<Ingredient>, IngredientRepository>();
            services.AddScoped<IRepository<IngredientDetail>, IngredientDetailRepository>();
            services.AddScoped<IRepository<Measure>, MeasureRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
