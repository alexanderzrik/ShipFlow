using FluentValidation;
using ShipFlow.Application.Features.Customers.Queries;

namespace ShipFlow.Application.Features.Customers.Validators;

/// <summary>
/// قواعد التحقق من صحة معاملات الاستعلام الخاصة بـGET /api/customers.
/// تحمي السيرفر من قيم ضخمة أو غير صالحة (pageSize عشوائي، sortBy غير موجود، إلخ)
/// وتضمن أن أي قيمة تصل إلى طبقة الـService هي قيمة معروفة ومقبولة.
/// </summary>
public class CustomerQueryParametersValidator : AbstractValidator<CustomerQueryParameters>
{
    /// <summary>
    /// الحقول المسموح الترتيب بها. أي قيمة خارج هذه القائمة تُرفَض بـ400.
    /// هذه White-list (قائمة بيضاء) — وليست Black-list — لأنها الأكثر أماناً:
    /// نسمح فقط بما نعرفه، ونرفض الباقي.
    /// </summary>
    private static readonly string[] AllowedSortFields =
    {
        "fullName",
        "phoneNumber",
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
    /// يُهيّئ قواعد التحقق لكل خاصية في <see cref="CustomerQueryParameters"/>.
    /// </summary>
    public CustomerQueryParametersValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("رقم الصفحة يجب أن يكون 1 أو أكبر.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("حجم الصفحة يجب أن يكون بين 1 و 100.");

        RuleFor(x => x.Search)
            .MaximumLength(200).WithMessage("نص البحث يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.SortBy)
            .NotEmpty().WithMessage("حقل الترتيب مطلوب.")
            .Must(BeAllowedSortField)
                .WithMessage("حقل الترتيب غير صالح. القيم المسموحة: fullName, phoneNumber, createdAt, updatedAt.");

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