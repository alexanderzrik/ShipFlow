using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.Shipments.Dtos;

/// <summary>
/// بيانات شحنة تُرسَل للعميل عند القراءة.
/// يحتوي PackageCount (عدد البضائع المحمّلة) — مفيد للعرض السريع في القوائم.
/// </summary>
public class ShipmentDto
{
    /// <summary>المعرف الفريد للشحنة.</summary>
    public int Id { get; set; }

    /// <summary>رقم الشحنة الفريد (مثل SHN-20261024-1234).</summary>
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
    public ShipmentStatus Status { get; set; }

    /// <summary>تاريخ الانطلاق الفعلي (null قبل المغادرة).</summary>
    public DateTime? DepartureDate { get; set; }

    /// <summary>تاريخ الوصول الفعلي (null قبل الوصول).</summary>
    public DateTime? ArrivalDate { get; set; }

    /// <summary>ملاحظات إضافية (اختياري).</summary>
    public string? Notes { get; set; }

    /// <summary>عدد البضائع المحمّلة في هذه الشحنة (محسوب من p.Packages.Count).</summary>
    public int PackageCount { get; set; }

    /// <summary>تاريخ إنشاء السجل (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>تاريخ آخر تحديث (UTC) — null إذا لم يُحدَّث.</summary>
    public DateTime? UpdatedAt { get; set; }
}