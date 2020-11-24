using FluentValidation;
using HomeTask4.Core.Entities;
namespace HomeTask4.Infrastructure.Validators
{
    public class RecipeStepValidator : AbstractValidator<RecipeStep>
    {
        public RecipeStepValidator()
        {
            RuleFor(x => x.Description).NotNull().Length(4, 600).WithMessage("Description must be {MinLength} to {MaxLength} characters long.");
        }
    }
}
