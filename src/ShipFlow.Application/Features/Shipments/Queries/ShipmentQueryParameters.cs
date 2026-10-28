using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.Shipments.Queries;

/// <summary>
/// معاملات الاستعلام الخاصة بـGET /api/shipments.
/// يستقبل قيم الـQuery String ويضعها ASP.NET Core في هذه الخصائص تلقائياً
/// عند تمريره كـ[FromQuery] في الـController.
/// </summary>
/// <remarks>
/// كل الخصائص لها قيم افتراضية منطقية، لذا يعمل الاستدعاء
/// GET /api/shipments (بدون أي Query) بشكل صحيح من أول مرة.
/// </remarks>
public class ShipmentQueryParameters
{
    /// <summary>
    /// رقم الصفحة المطلوبة (يبدأ من 1).
    /// الافتراضي: 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// عدد العناصر في كل صفحة.
    /// الافتراضي: 10. الحد الأقصى المسموح: 100.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// نص بحث اختياري — يُطابَق ضد ShipmentNumber أو DriverName (Contains).
    /// null أو نص فارغ = بدون فلترة بحث.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// فلترة حسب حالة الشحنة.
    /// null = الكل، قيمة محددة = تلك الحالة فقط.
    /// </summary>
    public ShipmentStatus? Status { get; set; }

    /// <summary>
    /// فلترة حسب مدينة الانطلاق (Contains، case-insensitive على مستوى SQL Server).
    /// null = الكل.
    /// </summary>
    public string? FromCity { get; set; }

    /// <summary>
    /// فلترة حسب مدينة الوصول (Contains).
    /// null = الكل.
    /// </summary>
    public string? ToCity { get; set; }

    /// <summary>
    /// حقل الترتيب. القيم المسموحة:
    /// shipmentNumber, fromCity, toCity, status, departureDate, arrivalDate, createdAt, updatedAt.
    /// الافتراضي: createdAt (الأحدث أولاً).
    /// يُتحقَّق من القيمة عبر الـValidator (white-list) لمنع أخطاء EF.
    /// </summary>
    public string SortBy { get; set; } = "createdAt";

    /// <summary>
    /// اتجاه الترتيب. القيم المسموحة: asc, desc (case-insensitive).
    /// الافتراضي: desc (تنازلي — الأحدث أولاً).
    /// </summary>
    public string SortOrder { get; set; } = "desc";
}