using ShipFlow.Domain.Enums;

namespace ShipFlow.Domain.Entities;

/// <summary>
/// يمثل شحنة (رحلة سيارة) تحمل مجموعة من البضائع.
/// </summary>
public class Shipment
{
    /// <summary>المعرف الفريد للشحنة.</summary>
    public int Id { get; set; }

    /// <summary>رقم الشحنة الفريد.</summary>
    public string ShipmentNumber { get; set; } = string.Empty;

    /// <summary>مدينة الانطلاق.</summary>
    public string FromCity { get; set; } = string.Empty;

    /// <summary>مدينة الوصول.</summary>
    public string ToCity { get; set; } = string.Empty;

    /// <summary>اسم السائق (اختياري).</summary>
    public string? DriverName { get; set; }

    /// <summary>رقم هاتف السائق (اختياري).</summary>
    public string? DriverPhone { get; set; }

    /// <summary>رقم المركبة (اختياري).</summary>
    public string? VehicleNumber { get; set; }

    /// <summary>حالة الشحنة الحالية.</summary>
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Loading;

    /// <summary>تاريخ الانطلاق الفعلي (اختياري).</summary>
    public DateTime? DepartureDate { get; set; }

    /// <summary>تاريخ الوصول الفعلي (اختياري).</summary>
    public DateTime? ArrivalDate { get; set; }

    /// <summary>ملاحظات إضافية (اختياري).</summary>
    public string? Notes { get; set; }

    /// <summary>تاريخ إنشاء السجل (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>تاريخ آخر تحديث (UTC).</summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation Property

    /// <summary>البضائع المحمّلة في هذه الشحنة.</summary>
    public ICollection<Package> Packages { get; set; } = new List<Package>();
}