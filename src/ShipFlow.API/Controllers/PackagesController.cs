using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.Packages.Dtos;
using ShipFlow.Application.Features.Packages.Queries;
using ShipFlow.Application.Features.Packages.Services;

namespace ShipFlow.API.Controllers;

/// <summary>
/// نقاط نهاية إدارة البضائع.
/// يعتمد فقط على IPackageService.
/// يتطلب توكن JWT صالحاً لجميع العمليات.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;

    /// <summary>
    /// ينشئ نسخة جديدة من Controller البضائع.
    /// </summary>
    public PackagesController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    /// <summary>
    /// يجلب صفحة من البضائع.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<PackageDto>>> GetAll(
        [FromQuery] PackageQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await _packageService.GetAllAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// يجلب بضاعة واحدة بمعرّفها.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PackageDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var package = await _packageService.GetByIdAsync(id, cancellationToken);
        return Ok(package);
    }

    /// <summary>
    /// ينشئ بضاعة جديدة.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PackageDto>> Create(
        [FromBody] CreatePackageRequest request,
        CancellationToken cancellationToken)
    {
        var package = await _packageService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = package.Id }, package);
    }

    /// <summary>
    /// يحدّث بيانات بضاعة موجودة.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PackageDto>> Update(
        int id,
        [FromBody] UpdatePackageRequest request,
        CancellationToken cancellationToken)
    {
        var package = await _packageService.UpdateAsync(id, request, cancellationToken);
        return Ok(package);
    }

    /// <summary>
    /// يحذف بضاعة بمعرّفها.
    /// يتطلب دور Admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _packageService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}