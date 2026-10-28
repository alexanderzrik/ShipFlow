namespace ShipFlow.Application.Features.BillOfLadings.Dtos;

/// <summary>
/// بيانات إنشاء بوليصة شحن جديدة.
/// لا يحتوي Id، CreatedAt، UpdatedAt — تُدار من السيرفر.
/// BillNumber اختياري: لو null/فارغ، يولّد السيرفر رقماً تلقائياً.
/// IssuedAt اختياري: لو null، يستخدم السيرفر UtcNow (يسمح بالـbackdating).
/// </summary>
public class CreateBillOfLadingRequest
{
    /// <summary>
    /// رقم البوليصة (اختياري، حتى 50 حرف).
    /// لو null/فارغ: يولّد السيرفر رقماً تلقائياً بصيغة BOL-yyyyMMddHHmmss-XXXX.
    /// </summary>
    public string? BillNumber { get; set; }

    /// <summary>
    /// تاريخ إصدار البوليصة (اختياري).
    /// لو null: يستخدم السيرفر DateTime.UtcNow.
    /// يسمح بإصدار وثيقة لتاريخ سابق (backdating) عند الحاجة.
    /// </summary>
    public DateTime? IssuedAt { get; set; }

    /// <summary>اسم الموظف الذي أصدر البوليصة (اختياري، حتى 200 حرف).</summary>
    public string? IssuedBy { get; set; }

    /// <summary>أجرة التوصيل (مطلوب، أكبر من أو يساوي صفر).</summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>ملاحظات إضافية (اختياري، حتى 1000 حرف).</summary>
    public string? Notes { get; set; }

    /// <summary>معرف البضاعة المرتبطة (مطلوب، يجب أن يكون موجوداً بدون بوليصة مسبقة).</summary>
    public int PackageId { get; set; }
}