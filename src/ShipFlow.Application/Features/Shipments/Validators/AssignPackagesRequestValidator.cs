using FluentValidation;
using ShipFlow.Application.Features.Shipments.Dtos;

namespace ShipFlow.Application.Features.Shipments.Validators;

/// <summary>
/// قواعد التحقق من صحة طلب إضافة بضائع إلى شحنة.
/// يضمن وجود بضاعة واحدة على الأقل وعدم وجود معرّفات مكررة.
/// </summary>
public class AssignPackagesRequestValidator : AbstractValidator<AssignPackagesRequest>
{
    /// <summary>
    /// يُهيّئ قواعد التحقق لـ<see cref="AssignPackagesRequest"/>.
    /// </summary>
    public AssignPackagesRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.PackageIds)
            .NotNull().WithMessage("قائمة البضائع مطلوبة.")
            .NotEmpty().WithMessage("يجب تمرير بضاعة واحدة على الأقل.")
            .Must(HaveNoDuplicates).WithMessage("قائمة البضائع تحتوي على معرّفات مكررة.");
    }

    /// <summary>
    /// يتحقق من عدم وجود معرّفات مكررة في القائمة.
    /// </summary>
    private static bool HaveNoDuplicates(List<int>? packageIds)
    {
        if (packageIds is null)
        {
            return true;
        }

        return packageIds.Distinct().Count() == packageIds.Count;
    }
}