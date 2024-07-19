using DataManager.Core.DBModels;
using FluentValidation;

namespace DataManager.Core.DbValidation;

public class DataModelTwoValidator : AbstractValidator<DataModelTwo>
{
    public DataModelTwoValidator()
    {
        RuleFor(d => d.PeriodStartDate)
            .NotEmpty()

            .Must(date => DbValidation.CheckIsValidDate(date))
            .WithMessage("PeriodStartDate must be in this format: dd.MM.yyyy.");

        RuleFor(d => d.PeriodEndDate)
            .NotEmpty()
            .WithMessage("PeriodEndDate must not be empty!")

            .Must(date => DbValidation.CheckIsValidDate(date))
            .WithMessage("PeriodEndDate must be in this format: dd.MM.yyyy. HH:mm:ss")

            .GreaterThan(d => d.PeriodStartDate.ToDateTime(new TimeOnly(0, 0, 0)))
            .WithMessage("PeriodEndDate must be greater than PeriodStartDate!");
    }
}