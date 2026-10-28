using FluentValidation;
using ShipFlow.Application.Features.Customers.Dtos;

namespace ShipFlow.Application.Features.Customers.Validators;

/// <summary>
/// قواعد التحقق من صحة بيانات تحديث زبون موجود.
/// مطابقة لقواعد <see cref="CreateCustomerRequestValidator"/> مع فارق واحد:
/// وجود حقل <see cref="UpdateCustomerRequest.IsActive"/> الذي لا يحتاج أي قاعدة
/// (نوعه bool ولا يقبل قيماً خارج true/false).
/// </summary>
public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    /// <summary>
    /// يُهيّئ قواعد التحقق لكل حقل في <see cref="UpdateCustomerRequest"/>.
    /// </summary>
    public UpdateCustomerRequestValidator()
    {
        // إيقاف التقييم بعد أول فشل داخل سلسلة القواعد لنفس الحقل.
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب.")
            .MaximumLength(200).WithMessage("الاسم الكامل يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب.")
            .MaximumLength(30).WithMessage("رقم الهاتف يجب ألا يتجاوز 30 حرف.");

        RuleFor(x => x.AlternativePhoneNumber)
            .MaximumLength(30).WithMessage("رقم الهاتف البديل يجب ألا يتجاوز 30 حرف.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("العنوان يجب ألا يتجاوز 500 حرف.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("الملاحظات يجب ألا تتجاوز 1000 حرف.");
    }
}