using HomeTask4.Core.Controllers;
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
            services.AddDbContext<Task4DBContext>(opts => opts.UseSqlServer(connectionString).UseLazyLoadingProxies());
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IUnitOfWork,UnitOfWork>();

            services.AddScoped<IngredientController>();
            services.AddScoped<CategoryController>();
            services.AddScoped<RecipeController>();
            services.AddScoped<RecipeStepController>();
            services.AddScoped<NavigationController>();
            return services;
        }
    }
}
