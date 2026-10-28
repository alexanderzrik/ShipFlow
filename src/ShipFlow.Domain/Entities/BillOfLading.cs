namespace ShipFlow.Domain.Entities;

/// <summary>
/// يمثل بوليصة شحن — مستند قانوني يصدر لكل بضاعة.
/// </summary>
public class BillOfLading
{
    /// <summary>المعرف الفريد للبوليصة.</summary>
    public int Id { get; set; }

    /// <summary>رقم البوليصة الفريد.</summary>
    public string BillNumber { get; set; } = string.Empty;

    /// <summary>تاريخ إصدار البوليصة (UTC).</summary>
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>اسم الموظف الذي أصدر البوليصة (اختياري).</summary>
    public string? IssuedBy { get; set; }

    /// <summary>أجرة التوصيل.</summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>ملاحظات إضافية (اختياري).</summary>
    public string? Notes { get; set; }

    /// <summary>تاريخ إنشاء السجل (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>تاريخ آخر تحديث (UTC).</summary>
    public DateTime? UpdatedAt { get; set; }

    // Foreign Key

    /// <summary>معرف البضاعة المرتبطة (علاقة 1:1).</summary>
    public int PackageId { get; set; }

    // Navigation Property

    /// <summary>البضاعة المرتبطة بالبوليصة.</summary>
    public Package Package { get; set; } = null!;
}