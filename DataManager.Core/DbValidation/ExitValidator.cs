using DataManager.Core.DBModels;
using FluentValidation;

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