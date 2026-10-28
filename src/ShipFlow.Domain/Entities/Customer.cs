namespace ShipFlow.Domain.Entities;

/// <summary>
/// يمثل زبوناً (فرد أو شركة) يقوم بشحن بضائع.
/// </summary>
public class Customer
{
    /// <summary>المعرف الفريد للزبون.</summary>
    public int Id { get; set; }

    /// <summary>الاسم الكامل للزبون (فرد أو شركة).</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>رقم الهاتف الرئيسي للتواصل.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>رقم هاتف بديل (اختياري).</summary>
    public string? AlternativePhoneNumber { get; set; }

    /// <summary>العنوان (اختياري).</summary>
    public string? Address { get; set; }

    /// <summary>ملاحظات إضافية (اختياري).</summary>
    public string? Notes { get; set; }

    /// <summary>هل الزبون نشط؟ (للأرشفة بدل الحذف).</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>تاريخ إنشاء السجل (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>تاريخ آخر تحديث (UTC) — null إذا ما تحدّث.</summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation Property

    /// <summary>البضائع التي يملكها هذا الزبون.</summary>
    public ICollection<Package> Packages { get; set; } = new List<Package>();
}