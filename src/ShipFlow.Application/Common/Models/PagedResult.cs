namespace ShipFlow.Application.Common.Models;

/// <summary>
/// يمثّل صفحة واحدة من نتائج مقسّمة (Pagination).
/// يحمل العناصر نفسها + معلومات مساعدة يحتاجها الـFrontend
/// (رقم الصفحة، الحجم، العدد الكلي، إجمالي الصفحات...).
/// </summary>
/// <typeparam name="T">نوع عناصر الصفحة (مثل CustomerDto).</typeparam>
public class PagedResult<T>
{
    /// <summary>عناصر الصفحة الحالية (بعد التطبيق Skip/Take).</summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>العدد الكلي للعناصر في قاعدة البيانات (قبل التطبيق Skip/Take).</summary>
    public int TotalCount { get; init; }

    /// <summary>رقم الصفحة الحالية (يبدأ من 1).</summary>
    public int Page { get; init; }

    /// <summary>عدد العناصر المطلوبة في كل صفحة.</summary>
    public int PageSize { get; init; }

    /// <summary>العدد الكلي للصفحات (محسوب: Ceil(TotalCount / PageSize)).</summary>
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>هل توجد صفحة سابقة؟ (Page > 1).</summary>
    public bool HasPrevious => Page > 1;

    /// <summary>هل توجد صفحة تالية؟ (Page أقل من TotalPages).</summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// ينشئ نسخة جديدة من نتيجة مُجزَّأة.
    /// </summary>
    /// <param name="items">عناصر الصفحة الحالية.</param>
    /// <param name="totalCount">العدد الكلي للعناصر قبل التقسيم.</param>
    /// <param name="page">رقم الصفحة الحالية.</param>
    /// <param name="pageSize">عدد العناصر في كل صفحة.</param>
    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    /// <summary>
    /// ينشئ نسخة فارغة (تُستخدم كقيمة افتراضية أو عند عدم وجود نتائج).
    /// </summary>
    public PagedResult()
    {
    }
}