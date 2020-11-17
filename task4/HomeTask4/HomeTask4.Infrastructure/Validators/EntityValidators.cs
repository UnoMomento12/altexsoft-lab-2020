using FluentValidation;
using HomeTask4.Core.Entities;

namespace HomeTask4.Infrastructure.Validators
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(4, 50).WithMessage("Name must be {MinLength} to {MaxLength} characters.");
        }
    }
    public class RecipeValidator : AbstractValidator<Recipe>
    {
        public RecipeValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(4, 50).WithMessage("Name must be {MinLength} to {MaxLength} characters long.");
        }
    }
    public class IngredientValidator : AbstractValidator<Ingredient>
    {
        public IngredientValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(4, 50).WithMessage("Name must be {MinLength} to {MaxLength} characters long.");
        }
    }
    public class RecipeStepValidator : AbstractValidator<RecipeStep>
    {
        public RecipeStepValidator()
        {
            RuleFor(x => x.Description).NotNull().Length(4, 600).WithMessage("Description must be {MinLength} to {MaxLength} characters long.");
        }
    }
    public class MeasureValidator : AbstractValidator<Measure>
    {
        public MeasureValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(0, 50).WithMessage("Name must be {MinLength} to {MaxLength} characters long.");
        }
    }
    public class IngredientDetailValidator : AbstractValidator<IngredientDetail>
    {
        public IngredientDetailValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
        }
    }
}
