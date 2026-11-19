using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.Currencies.Dtos;

/// <summary>
/// بيانات سعر صرف عملة.
/// </summary>
public class CurrencyRateDto
{
    public int Id { get; set; }
    public Currency Currency { get; set; }
    public decimal RateToSyp { get; set; }
    public DateTime UpdatedAt { get; set; }
}