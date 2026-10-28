namespace ShipFlow.Application.Features.BillOfLadings.Dtos;

/// <summary>
/// بيانات تحديث بوليصة شحن موجودة.
/// لا يحتوي BillNumber (ثابت = هوية الوثيقة)،
/// ولا PackageId (نقل الملكية نادر ويتم بإصدار بوليصة جديدة)،
/// ولا IssuedAt (تاريخ الإصدار لا يُعدَّل).
/// </summary>
public class UpdateBillOfLadingRequest
{
    /// <summary>اسم الموظف الذي أصدر البوليصة (اختياري، حتى 200 حرف).</summary>
    public string? IssuedBy { get; set; }

    /// <summary>أجرة التوصيل (مطلوب، أكبر من أو يساوي صفر).</summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>ملاحظات إضافية (اختياري، حتى 1000 حرف).</summary>
    public string? Notes { get; set; }
}