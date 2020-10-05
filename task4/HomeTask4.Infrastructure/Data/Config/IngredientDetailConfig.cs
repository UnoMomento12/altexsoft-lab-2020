using HomeTask4.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTask4.Infrastructure.Data.Config
{
    public class IngredientDetailConfig : IEntityTypeConfiguration<IngredientDetail>
    {
        public void Configure(EntityTypeBuilder<IngredientDetail> builder)
        {
            builder.Ignore("Id");
            builder.HasKey(x => new { x.RecipeId, x.IngredientId });
            builder.Property(x => x.Amount).IsRequired().HasDefaultValue(0);
        }
    }
}
