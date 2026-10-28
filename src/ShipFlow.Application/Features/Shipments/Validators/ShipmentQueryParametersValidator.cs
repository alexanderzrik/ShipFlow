using FluentValidation;
using ShipFlow.Application.Features.Shipments.Queries;

namespace ShipFlow.Application.Features.Shipments.Validators;

/// <summary>
/// قواعد التحقق من صحة معاملات الاستعلام الخاصة بـGET /api/shipments.
/// تحمي السيرفر من قيم ضخمة أو غير صالحة (pageSize عشوائي، sortBy غير موجود... إلخ).
/// </summary>
public class ShipmentQueryParametersValidator : AbstractValidator<ShipmentQueryParameters>
{
    /// <summary>
    /// الحقول المسموح الترتيب بها (White-list). أي قيمة خارجها تُرفض بـ400.
    /// </summary>
    private static readonly string[] AllowedSortFields =
    {
        "shipmentNumber",
        "fromCity",
        "toCity",
        "status",
        "departureDate",
        "arrivalDate",
        "createdAt",
        "updatedAt"
    };

    /// <summary>
    /// القيم المسموحة لاتجاه الترتيب.
    /// </summary>
    private static readonly string[] AllowedSortOrders =
    {
        "asc",
        "desc"
    };

    /// <summary>
    /// يُهيّئ قواعد التحقق لكل خاصية في <see cref="ShipmentQueryParameters"/>.
    /// </summary>
    public ShipmentQueryParametersValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("رقم الصفحة يجب أن يكون 1 أو أكبر.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("حجم الصفحة يجب أن يكون بين 1 و 100.");

        RuleFor(x => x.Search)
            .MaximumLength(200).WithMessage("نص البحث يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.FromCity)
            .MaximumLength(100).WithMessage("مدينة الانطلاق يجب ألا تتجاوز 100 حرف.");

        RuleFor(x => x.ToCity)
            .MaximumLength(100).WithMessage("مدينة الوصول يجب ألا تتجاوز 100 حرف.");

        RuleFor(x => x.SortBy)
            .NotEmpty().WithMessage("حقل الترتيب مطلوب.")
            .Must(BeAllowedSortField)
                .WithMessage("حقل الترتيب غير صالح. القيم المسموحة: shipmentNumber, fromCity, toCity, status, departureDate, arrivalDate, createdAt, updatedAt.");

        RuleFor(x => x.SortOrder)
            .NotEmpty().WithMessage("اتجاه الترتيب مطلوب.")
            .Must(BeAllowedSortOrder)
                .WithMessage("اتجاه الترتيب غير صالح. القيم المسموحة: asc, desc.");
    }

    /// <summary>
    /// يتحقق أن <paramref name="sortBy"/> من ضمن الحقول المسموحة (مقارنة case-insensitive).
    /// </summary>
    private static bool BeAllowedSortField(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return false;
        }

        return AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// يتحقق أن <paramref name="sortOrder"/> من ضمن القيم المسموحة (مقارنة case-insensitive).
    /// </summary>
    private static bool BeAllowedSortOrder(string? sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortOrder))
        {
            return false;
        }

        return AllowedSortOrders.Contains(sortOrder, StringComparer.OrdinalIgnoreCase);
    }
}