using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.Shipments.Dtos;

/// <summary>
/// بيانات تحديث شحنة موجودة.
/// لا يحتوي ShipmentNumber (ثابت بعد الإنشاء = هوية الشحنة)،
/// ولا PackageCount (محسوب)، ولا DepartureDate/ArrivalDate (يُملأان تلقائياً حسب Status).
/// Status مسموح هنا، لكن الـService يفرض قيد "للأمام فقط".
/// </summary>
public class UpdateShipmentRequest
{
    /// <summary>مدينة الانطلاق (مطلوب، حتى 100 حرف).</summary>
    public string FromCity { get; set; } = string.Empty;

    /// <summary>مدينة الوصول (مطلوب، حتى 100 حرف).</summary>
    public string ToCity { get; set; } = string.Empty;

    /// <summary>اسم السائق (اختياري، حتى 200 حرف).</summary>
    public string? DriverName { get; set; }

    /// <summary>رقم هاتف السائق (اختياري، حتى 30 حرف).</summary>
    public string? DriverPhone { get; set; }

    /// <summary>رقم المركبة (اختياري، حتى 50 حرف).</summary>
    public string? VehicleNumber { get; set; }

    /// <summary>
    /// حالة الشحنة الجديدة (اختياري — null يعني عدم التغيير).
    /// الـService يفرض "للأمام فقط" (Loading → Departed → InTransit → Arrived → Closed).
    /// </summary>
    public ShipmentStatus? Status { get; set; }

    /// <summary>ملاحظات إضافية (اختياري، حتى 1000 حرف).</summary>
    public string? Notes { get; set; }
}