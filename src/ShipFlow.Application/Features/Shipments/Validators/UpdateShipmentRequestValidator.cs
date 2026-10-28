using FluentValidation;
using ShipFlow.Application.Features.Shipments.Dtos;

namespace ShipFlow.Application.Features.Shipments.Validators;

/// <summary>
/// قواعد التحقق من صحة بيانات تحديث شحنة موجودة.
/// مطابقة لقواعد <see cref="CreateShipmentRequestValidator"/> عدا ShipmentNumber
/// (ثابت بعد الإنشاء). Status لا قاعدة عليه هنا — Model Binding يتحقق من صحته،
/// ومنطق "للأمام فقط" يُفرض في طبقة الـService (يعتمد على الحالة الحالية في DB).
/// </summary>
public class UpdateShipmentRequestValidator : AbstractValidator<UpdateShipmentRequest>
{
    /// <summary>
    /// يُهيّئ قواعد التحقق لكل حقل في <see cref="UpdateShipmentRequest"/>.
    /// </summary>
    public UpdateShipmentRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FromCity)
            .NotEmpty().WithMessage("مدينة الانطلاق مطلوبة.")
            .MaximumLength(100).WithMessage("مدينة الانطلاق يجب ألا تتجاوز 100 حرف.");

        RuleFor(x => x.ToCity)
            .NotEmpty().WithMessage("مدينة الوصول مطلوبة.")
            .MaximumLength(100).WithMessage("مدينة الوصول يجب ألا تتجاوز 100 حرف.");

        RuleFor(x => x.DriverName)
            .MaximumLength(200).WithMessage("اسم السائق يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.DriverPhone)
            .MaximumLength(30).WithMessage("رقم هاتف السائق يجب ألا يتجاوز 30 حرف.");

        RuleFor(x => x.VehicleNumber)
            .MaximumLength(50).WithMessage("رقم المركبة يجب ألا يتجاوز 50 حرف.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("الملاحظات يجب ألا تتجاوز 1000 حرف.");
    }
}