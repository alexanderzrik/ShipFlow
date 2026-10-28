namespace ShipFlow.Application.Features.BillOfLadings.Dtos;

/// <summary>
/// بيانات بوليصة شحن تُرسَل للعميل عند القراءة.
/// تحتوي PackageTrackingNumber, ReceiverName, PackageDescription
/// (من JOIN مع Packages) — مفيدة للعرض بدون طلب إضافي.
/// </summary>
public class BillOfLadingDto
{
    /// <summary>المعرف الفريد للبوليصة.</summary>
    public int Id { get; set; }

    /// <summary>رقم البوليصة الفريد (مثل BOL-20261024-1234).</summary>
    public string BillNumber { get; set; } = string.Empty;

    /// <summary>تاريخ إصدار البوليصة (UTC).</summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>اسم الموظف الذي أصدر البوليصة (اختياري).</summary>
    public string? IssuedBy { get; set; }

    /// <summary>أجرة التوصيل.</summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>ملاحظات إضافية (اختياري).</summary>
    public string? Notes { get; set; }

    // معلومات البضاعة المرتبطة (JOIN)

    /// <summary>معرف البضاعة المرتبطة (علاقة 1:1).</summary>
    public int PackageId { get; set; }

    /// <summary>رقم تتبع البضاعة (من JOIN).</summary>
    public string PackageTrackingNumber { get; set; } = string.Empty;

    /// <summary>وصف البضاعة (من JOIN).</summary>
    public string PackageDescription { get; set; } = string.Empty;

    /// <summary>اسم مستلم البضاعة (من JOIN).</summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>معرف الزبون المالك للبضاعة (من JOIN).</summary>
    public int CustomerId { get; set; }

    // طوابع زمنية

    /// <summary>تاريخ إنشاء السجل (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>تاريخ آخر تحديث (UTC) — null إذا لم يُحدَّث.</summary>
    public DateTime? UpdatedAt { get; set; }
}