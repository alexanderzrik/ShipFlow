namespace ShipFlow.Application.Features.Customers.Dtos;

/// <summary>
/// بيانات الزبون التي تُرجع للـClient.
/// نسخة مُبسّطة من كيان Customer بدون navigation properties.
/// </summary>
public class CustomerDto
{
    /// <summary>المعرف الفريد للزبون.</summary>
    public int Id { get; set; }

    /// <summary>الاسم الكامل.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>رقم الهاتف الرئيسي.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>رقم هاتف بديل (اختياري).</summary>
    public string? AlternativePhoneNumber { get; set; }

    /// <summary>العنوان (اختياري).</summary>
    public string? Address { get; set; }

    /// <summary>ملاحظات (اختياري).</summary>
    public string? Notes { get; set; }

    /// <summary>هل الزبون نشط؟</summary>
    public bool IsActive { get; set; }

    /// <summary>تاريخ الإنشاء (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>تاريخ آخر تحديث (UTC) — null إذا لم يُحدّث.</summary>
    public DateTime? UpdatedAt { get; set; }
}