using FluentValidation;
using HomeTask4.Core.Entities;

namespace HomeTask4.Infrastructure.Validators
{
    public class IngredientDetailValidator : AbstractValidator<IngredientDetail>
    {
        public IngredientDetailValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
        }
    }
}
