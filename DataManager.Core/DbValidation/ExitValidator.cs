using DataManager.Core.DBModels;
using FluentValidation;

namespace DataManager.Core.DbValidation
{
    public class ExitValidator : AbstractValidator<Exit>
    {
        public ExitValidator(IEnumerable<Exit> allExits)
        {
            RuleFor(e => e.Name)
                .NotEmpty()
                .Must((exit, name) => !allExits.Any(e => e.Name == name && e.Id != exit.Id))
                .WithMessage("ExitName must be unique!");
        }
    }
}