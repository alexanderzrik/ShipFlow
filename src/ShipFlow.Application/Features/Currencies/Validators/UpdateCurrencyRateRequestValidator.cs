using FluentValidation;
using ShipFlow.Application.Features.Currencies.Dtos;

namespace ShipFlow.Application.Features.Currencies.Validators;

/// <summary>
/// قواعد التحقق من صحة طلب تحديث سعر الصرف.
/// </summary>
public class UpdateCurrencyRateRequestValidator : AbstractValidator<UpdateCurrencyRateRequest>
{
    public UpdateCurrencyRateRequestValidator()
    {
        RuleFor(x => x.RateToSyp)
            .GreaterThan(0).WithMessage("سعر الصرف يجب أن يكون أكبر من صفر.");
    }
}