using FluentValidation;
using UnitConversion.Model.Dtos.Requests;

namespace UnitConversion.Service.Conversion.Validators;

public class ConversionRequestValidator : AbstractValidator<ConversionRequest>
{
    public ConversionRequestValidator()
    {
        RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required.");
        RuleFor(x => x.FromUnit).NotEmpty().WithMessage("FromUnit is required.");
        RuleFor(x => x.ToUnit).NotEmpty().WithMessage("ToUnit is required.");
        RuleFor(x => x.Value)
            .Must(v => !double.IsNaN(v) && !double.IsInfinity(v))
            .WithMessage("Value must be a finite number.");
    }
}
