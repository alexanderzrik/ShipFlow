namespace ShipFlow.Application.Features.BillOfLadings.Queries;

/// <summary>
/// معاملات الاستعلام الخاصة بـGET /api/billofladings.
/// يستقبل قيم الـQuery String ويضعها ASP.NET Core في هذه الخصائص تلقائياً
/// عند تمريره كـ[FromQuery] في الـController.
/// </summary>
/// <remarks>
/// كل الخصائص لها قيم افتراضية منطقية، لذا يعمل الاستدعاء
/// GET /api/billofladings (بدون أي Query) بشكل صحيح من أول مرة.
/// </remarks>
public class BillOfLadingQueryParameters
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
    /// نص بحث اختياري — يُطابَق ضد BillNumber أو IssuedBy (Contains).
    /// null أو نص فارغ = بدون فلترة بحث.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// فلترة حسب البضاعة المرتبطة.
    /// null = الكل، قيمة محددة = بوليصة تلك البضاعة فقط (0 أو 1 نتيجة بسبب 1:1).
    /// </summary>
    public int? PackageId { get; set; }

    /// <summary>
    /// حقل الترتيب. القيم المسموحة:
    /// billNumber, issuedAt, deliveryFee, createdAt, updatedAt.
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