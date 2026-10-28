namespace ShipFlow.Application.Features.Customers.Queries;

/// <summary>
/// معاملات الاستعلام الخاصة بـGET /api/customers.
/// يستقبل قيم الـQuery String ويضعها ASP.NET Core في هذه الخصائص تلقائياً
/// عند تمريره كـ[FromQuery] في الـController.
/// </summary>
/// <remarks>
/// كل الخصائص لها قيم افتراضية منطقية، لذا يعمل الاستدعاء
/// GET /api/customers (بدون أي Query) بشكل صحيح من أول مرة.
/// </remarks>
public class CustomerQueryParameters
{
    /// <summary>
    /// رقم الصفحة المطلوبة (يبدأ من 1).
    /// الافتراضي: 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// عدد العناصر في كل صفحة.
    /// الافتراضي: 10. الحد الأقصى المسموح: 100 (يُفرض عبر الـValidator).
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// نص بحث اختياري — يُطابَق ضد FullName أو PhoneNumber (Contains).
    /// null أو نص فارغ = بدون فلترة بحث.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// فلترة حسب حالة التنشيط.
    /// true = النشطين فقط، false = المؤرشفين فقط، null = الكل (بدون فلترة).
    /// الافتراضي: null.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// حقل الترتيب. القيم المسموحة: fullName, phoneNumber, createdAt, updatedAt.
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