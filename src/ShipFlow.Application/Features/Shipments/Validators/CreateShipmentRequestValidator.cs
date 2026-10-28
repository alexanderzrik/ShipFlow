using FluentValidation;
using ShipFlow.Application.Features.Shipments.Dtos;

namespace ShipFlow.Application.Features.Shipments.Validators;

/// <summary>
/// قواعد التحقق من صحة بيانات إنشاء شحنة جديدة.
/// تعكس القيود المعرّفة في ShipmentConfiguration (MaxLength + Required).
/// ShipmentNumber اختياري — لو مُرّر، يُفحص طوله فقط؛ التفرّد يُفحص في طبقة الـService.
/// </summary>
public class CreateShipmentRequestValidator : AbstractValidator<CreateShipmentRequest>
{
    /// <summary>
    /// يُهيّئ قواعد التحقق لكل حقل في <see cref="CreateShipmentRequest"/>.
    /// </summary>
    public CreateShipmentRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // ShipmentNumber اختياري — لو مُرّر، نتأكد أن طوله لا يتجاوز 50.
        // Empty/null مسموحان (السيرفر يولّد رقماً).
        RuleFor(x => x.ShipmentNumber)
            .MaximumLength(50).WithMessage("رقم الشحنة يجب ألا يتجاوز 50 حرف.");

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