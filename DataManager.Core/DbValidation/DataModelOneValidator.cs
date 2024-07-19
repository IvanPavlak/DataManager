using DataManager.Core.DBModels;
using FluentValidation;

namespace DataManager.Core.DbValidation;

public class DataModelOneValidator : AbstractValidator<DataModelOne>
{
    public DataModelOneValidator()
    {
        RuleFor(d => d.Exit)
            .NotEmpty();

        RuleFor(d => d.Port)
            .NotEmpty();

        RuleFor(d => d.UserGroup)
            .NotEmpty();

        RuleFor(d => d.Country)
            .NotEmpty();

        RuleFor(d => d.MemberId)
            .NotEmpty();

        RuleFor(d => d.Date)
            .NotEmpty()

            .Must(date => DbValidation.CheckIsValidDate(date, "yyyy-MM-dd"))
            .WithMessage("MeasurementDate must be in this format: yyyy-MM-dd");

        RuleFor(d => d.GainAmountOne)
            .GreaterThanOrEqualTo(0);

        RuleFor(d => d.GainAmountTwo)
            .GreaterThanOrEqualTo(0);

        RuleFor(d => d.Loss)
            .GreaterThanOrEqualTo(0);

        RuleFor(d => d.Total)
            .GreaterThanOrEqualTo(0);
    }
}