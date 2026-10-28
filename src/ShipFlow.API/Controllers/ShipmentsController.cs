using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.Packages.Dtos;
using ShipFlow.Application.Features.Shipments.Dtos;
using ShipFlow.Application.Features.Shipments.Queries;
using ShipFlow.Application.Features.Shipments.Services;

namespace ShipFlow.API.Controllers;

/// <summary>
/// نقاط نهاية إدارة الشحنات.
/// يعتمد فقط على IShipmentService.
/// يتطلب توكن JWT صالحاً لجميع العمليات.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    /// <summary>
    /// ينشئ نسخة جديدة من Controller الشحنات.
    /// </summary>
    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    /// <summary>
    /// يجلب صفحة من الشحنات.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ShipmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<ShipmentDto>>> GetAll(
        [FromQuery] ShipmentQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetAllAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// يجلب شحنة واحدة.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ShipmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.GetByIdAsync(id, cancellationToken);
        return Ok(shipment);
    }

    /// <summary>
    /// ينشئ شحنة جديدة.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ShipmentDto>> Create(
        [FromBody] CreateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
    }

    /// <summary>
    /// يحدّث شحنة موجودة.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ShipmentDto>> Update(
        int id,
        [FromBody] UpdateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.UpdateAsync(id, request, cancellationToken);
        return Ok(shipment);
    }

    /// <summary>
    /// يحذف شحنة.
    /// يتطلب دور Admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _shipmentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// يضيف بضائع لشحنة.
    /// </summary>
    [HttpPost("{id:int}/packages")]
    [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ShipmentDto>> AssignPackages(
        int id,
        [FromBody] AssignPackagesRequest request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.AssignPackagesAsync(id, request, cancellationToken);
        return Ok(shipment);
    }

    /// <summary>
    /// يزيل بضاعة من شحنة.
    /// </summary>
    [HttpDelete("{id:int}/packages/{packageId:int}")]
    [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ShipmentDto>> UnassignPackage(
        int id,
        int packageId,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.UnassignPackageAsync(id, packageId, cancellationToken);
        return Ok(shipment);
    }

    /// <summary>
    /// يجلب بضائع شحنة.
    /// </summary>
    [HttpGet("{id:int}/packages")]
    [ProducesResponseType(typeof(IReadOnlyList<PackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<PackageDto>>> GetPackages(
        int id,
        CancellationToken cancellationToken)
    {
        var packages = await _shipmentService.GetPackagesAsync(id, cancellationToken);
        return Ok(packages);
    }
}