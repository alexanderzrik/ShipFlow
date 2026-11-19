using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.Packages.Dtos;
using ShipFlow.Application.Features.Packages.Queries;
using ShipFlow.Application.Features.Packages.Services;

namespace ShipFlow.API.Controllers;

/// <summary>
/// نقاط نهاية إدارة البضائع.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;

    public PackagesController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PackageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PackageDto>>> GetAll([FromQuery] PackageQueryParameters query, CancellationToken cancellationToken)
    {
        return Ok(await _packageService.GetAllAsync(query, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PackageDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _packageService.GetByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// تتبع بضاعة برقم التتبع (متاح للجميع بدون تسجيل دخول).
    /// </summary>
    [AllowAnonymous]
    [HttpGet("track/{trackingNumber}")]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PackageDto>> Track(string trackingNumber, CancellationToken cancellationToken)
    {
        return Ok(await _packageService.GetByTrackingNumberAsync(trackingNumber, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PackageDto>> Create([FromBody] CreatePackageRequest request, CancellationToken cancellationToken)
    {
        var package = await _packageService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = package.Id }, package);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PackageDto>> Update(int id, [FromBody] UpdatePackageRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _packageService.UpdateAsync(id, request, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _packageService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// تسجيل استلام وتوزيع البضاعة للزبون.
    /// </summary>
    [HttpPatch("{id:int}/deliver")]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PackageDto>> Deliver(int id, [FromBody] DeliverPackageRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _packageService.DeliverAsync(id, request, cancellationToken));
    }
}