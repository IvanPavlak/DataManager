using FluentValidation;
using DataManager.Core.DBModels;

namespace DataManager.Core.DbValidation
{
    public class ExitValidator : AbstractValidator<Exit>
    {
        public ExitValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty();
        }
    }
}