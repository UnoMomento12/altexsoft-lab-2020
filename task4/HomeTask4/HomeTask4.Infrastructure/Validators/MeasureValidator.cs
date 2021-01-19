using FluentValidation;
using HomeTask4.Core.Entities;

namespace HomeTask4.Infrastructure.Validators
{
    public class MeasureValidator : AbstractValidator<Measure>
    {
        public MeasureValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(0, 50).WithMessage("Name must be {MinLength} to {MaxLength} characters long.");
        }
    }
}
