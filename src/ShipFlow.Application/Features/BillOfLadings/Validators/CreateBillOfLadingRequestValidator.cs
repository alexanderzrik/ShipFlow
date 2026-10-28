using FluentValidation;
using ShipFlow.Application.Features.BillOfLadings.Dtos;

namespace ShipFlow.Application.Features.BillOfLadings.Validators;

/// <summary>
/// قواعد التحقق من صحة بيانات إنشاء بوليصة شحن جديدة.
/// تعكس القيود المعرّفة في BillOfLadingConfiguration (MaxLength + Required).
/// BillNumber اختياري — لو مُرّر، يُفحص طوله فقط؛ التفرّد يُفحص في طبقة الـService.
/// التحقق من وجود البضاعة وعدم وجود بوليصة سابقة لها يُتمّ في الـService.
/// </summary>
public class CreateBillOfLadingRequestValidator : AbstractValidator<CreateBillOfLadingRequest>
{
    /// <summary>
    /// يُهيّئ قواعد التحقق لكل حقل في <see cref="CreateBillOfLadingRequest"/>.
    /// </summary>
    public CreateBillOfLadingRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // BillNumber اختياري — لو مُرّر، نتأكد أن طوله لا يتجاوز 50.
        RuleFor(x => x.BillNumber)
            .MaximumLength(50).WithMessage("رقم البوليصة يجب ألا يتجاوز 50 حرف.");

        RuleFor(x => x.IssuedBy)
            .MaximumLength(200).WithMessage("اسم المصدر يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.DeliveryFee)
            .GreaterThanOrEqualTo(0).WithMessage("أجرة التوصيل يجب أن تكون صفراً أو أكبر.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("الملاحظات يجب ألا تتجاوز 1000 حرف.");

        RuleFor(x => x.PackageId)
            .GreaterThan(0).WithMessage("معرّف البضاعة يجب أن يكون أكبر من صفر.");
    }
}