using ShipFlow.Application.Features.Currencies.Dtos;

namespace ShipFlow.Application.Features.Currencies.Services;

/// <summary>
/// خدمة إدارة أسعار العملات.
/// </summary>
public interface ICurrencyRateService
{
    /// <summary>يجلب كل أسعار العملات.</summary>
    Task<IReadOnlyList<CurrencyRateDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>يقوم بتحديث سعر صرف عملة معينة (أو إنشائها إذا لم تكن موجودة).</summary>
    Task<CurrencyRateDto> UpdateAsync(UpdateCurrencyRateRequest request, CancellationToken cancellationToken = default);
}