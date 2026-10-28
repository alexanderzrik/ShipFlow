using ShipFlow.Domain.Enums;

namespace ShipFlow.Domain.Entities;

/// <summary>
/// يمثل بضاعة/طرد واحد يُشحن من زبون إلى وجهة.
/// </summary>
public class Package
{
    /// <summary>المعرف الفريد للبضاعة.</summary>
    public int Id { get; set; }

    /// <summary>رقم التتبع الفريد للبضاعة.</summary>
    public string TrackingNumber { get; set; } = string.Empty;

    /// <summary>وصف محتوى البضاعة.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>وزن البضاعة بالكيلوغرام.</summary>
    public decimal Weight { get; set; }

    /// <summary>حالة البضاعة الحالية.</summary>
    public PackageStatus Status { get; set; } = PackageStatus.Received;

    /// <summary>اسم المستلم في الوجهة.</summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>رقم هاتف المستلم.</summary>
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>مدينة الوجهة.</summary>
    public string DestinationCity { get; set; } = string.Empty;

    /// <summary>ملاحظات إضافية (اختياري).</summary>
    public string? Notes { get; set; }

    /// <summary>تاريخ إنشاء السجل (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>تاريخ آخر تحديث (UTC) — null إذا ما تحدّث.</summary>
    public DateTime? UpdatedAt { get; set; }

    // Foreign Keys

    /// <summary>معرف الزبون المالك للبضاعة.</summary>
    public int CustomerId { get; set; }

    /// <summary>معرف الشحنة (اختياري — البضاعة يمكن أن تكون بدون شحنة).</summary>
    public int? ShipmentId { get; set; }

    // Navigation Properties

    /// <summary>الزبون المالك للبضاعة.</summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>الشحنة التي تحمل البضاعة (إن وُجدت).</summary>
    public Shipment? Shipment { get; set; }

    /// <summary>البوليصة الخاصة بالبضاعة (إن صدرت).</summary>
    public BillOfLading? BillOfLading { get; set; }
}