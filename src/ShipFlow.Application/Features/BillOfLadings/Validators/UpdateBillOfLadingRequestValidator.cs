using FluentValidation;
using ShipFlow.Application.Features.BillOfLadings.Dtos;

namespace ShipFlow.Application.Features.BillOfLadings.Validators;

/// <summary>
/// قواعد التحقق من صحة بيانات تحديث بوليصة شحن موجودة.
/// مطابقة لقواعد <see cref="CreateBillOfLadingRequestValidator"/> عدا الحقول
/// التي لا توجد في Update (BillNumber, IssuedAt, PackageId).
/// </summary>
public class UpdateBillOfLadingRequestValidator : AbstractValidator<UpdateBillOfLadingRequest>
{
    /// <summary>
    /// يُهيّئ قواعد التحقق لكل حقل في <see cref="UpdateBillOfLadingRequest"/>.
    /// </summary>
    public UpdateBillOfLadingRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.IssuedBy)
            .MaximumLength(200).WithMessage("اسم المصدر يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.DeliveryFee)
            .GreaterThanOrEqualTo(0).WithMessage("أجرة التوصيل يجب أن تكون صفراً أو أكبر.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("الملاحظات يجب ألا تتجاوز 1000 حرف.");
    }
}