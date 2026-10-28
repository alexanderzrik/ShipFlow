namespace ShipFlow.Application.Features.Shipments.Dtos;

/// <summary>
/// بيانات إنشاء شحنة جديدة.
/// لا يحتوي Id، Status، DepartureDate، ArrivalDate، CreatedAt، UpdatedAt، PackageCount
/// — تُدار كلها من السيرفر أو تُملأ لاحقاً.
/// ShipmentNumber اختياري: لو لم يُمرَّر، يولّده السيرفر تلقائياً.
/// </summary>
public class CreateShipmentRequest
{
    /// <summary>
    /// رقم الشحنة (اختياري، حتى 50 حرف).
    /// لو null/فارغ: يولّد السيرفر رقماً تلقائياً بصيغة SHN-yyyyMMddHHmmss-XXXX.
    /// </summary>
    public string? ShipmentNumber { get; set; }

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

    /// <summary>ملاحظات إضافية (اختياري، حتى 1000 حرف).</summary>
    public string? Notes { get; set; }
}