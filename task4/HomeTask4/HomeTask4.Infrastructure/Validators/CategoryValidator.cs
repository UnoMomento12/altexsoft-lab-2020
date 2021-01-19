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
}
