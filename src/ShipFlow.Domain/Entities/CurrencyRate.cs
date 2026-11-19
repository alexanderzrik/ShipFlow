using ShipFlow.Domain.Enums;

namespace ShipFlow.Domain.Entities;

/// <summary>
/// يمثل سعر صرف عملة أجنبية مقابل الليرة السورية.
/// </summary>
public class CurrencyRate
{
    /// <summary>المعرف الفريد.</summary>
    public int Id { get; set; }

    /// <summary>نوع العملة (USD, EUR, TRY).</summary>
    public Currency Currency { get; set; }

    /// <summary>سعر الصرف مقابل الليرة السورية (مثلاً: 1 دولار = 15000 ل.س).</summary>
    public decimal RateToSyp { get; set; }

    /// <summary>تاريخ آخر تحديث للسعر (UTC).</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}