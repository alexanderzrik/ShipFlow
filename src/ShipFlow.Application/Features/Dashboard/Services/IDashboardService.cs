using ShipFlow.Application.Features.Dashboard.Dtos;

namespace ShipFlow.Application.Features.Dashboard.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}