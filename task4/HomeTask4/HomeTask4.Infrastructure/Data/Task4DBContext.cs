using HomeTask4.Core.Entities;
using HomeTask4.Infrastructure.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace HomeTask4.Infrastructure.Data
{
    public class Task4DBContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Measure> Measures { get; set; }
        public DbSet<IngredientDetail> IngredientDetails { get; set; }
        public Task4DBContext(DbContextOptions<Task4DBContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new RecipeConfig());
            modelBuilder.ApplyConfiguration(new RecipeStepConfig());
            modelBuilder.ApplyConfiguration(new IngredientConfig());
            modelBuilder.ApplyConfiguration(new IngredientDetailConfig());
            modelBuilder.ApplyConfiguration(new MeasureConfig());
        }
    }
}
