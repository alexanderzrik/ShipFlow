using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Application.Features.Dashboard.Dtos;
using ShipFlow.Application.Features.Dashboard.Services;

namespace ShipFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _dashboardService.GetSummaryAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            // إرجاع الخطأ الحقيقي للواجهة لمعرفة السبب
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}