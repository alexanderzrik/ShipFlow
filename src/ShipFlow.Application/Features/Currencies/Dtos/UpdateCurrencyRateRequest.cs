using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.Currencies.Dtos;

/// <summary>
/// طلب تحديث سعر صرف عملة.
/// </summary>
public class UpdateCurrencyRateRequest
{
    /// <summary>العملة المطلوب تحديث سعرها.</summary>
    public Currency Currency { get; set; }

    /// <summary>سعر الصرف الجديد مقابل الليرة السورية (يجب أن يكون أكبر من صفر).</summary>
    public decimal RateToSyp { get; set; }
}