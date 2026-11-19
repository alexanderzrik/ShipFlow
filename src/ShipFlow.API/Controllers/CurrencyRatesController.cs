using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Application.Features.Currencies.Dtos;
using ShipFlow.Application.Features.Currencies.Services;

namespace ShipFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CurrencyRatesController : ControllerBase
{
    private readonly ICurrencyRateService _currencyRateService;

    public CurrencyRatesController(ICurrencyRateService currencyRateService)
    {
        _currencyRateService = currencyRateService;
    }

    /// <summary>
    /// جلب كل أسعار العملات.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CurrencyRateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CurrencyRateDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _currencyRateService.GetAllAsync(cancellationToken));
    }

    /// <summary>
    /// تحديث سعر صرف عملة (يتطلب دور Admin).
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut]
    [ProducesResponseType(typeof(CurrencyRateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CurrencyRateDto>> Update([FromBody] UpdateCurrencyRateRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _currencyRateService.UpdateAsync(request, cancellationToken));
    }
}