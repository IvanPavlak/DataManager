using FluentValidation;
using DataManager.Core.DBModels;

namespace DataManager.Core.DbValidation;

public class ModelOneValidator : AbstractValidator<ModelOne>
{
    public ModelOneValidator()
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