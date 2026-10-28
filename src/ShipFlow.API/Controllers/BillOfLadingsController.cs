using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.BillOfLadings.Dtos;
using ShipFlow.Application.Features.BillOfLadings.Queries;
using ShipFlow.Application.Features.BillOfLadings.Services;

namespace ShipFlow.API.Controllers;

/// <summary>
/// نقاط نهاية إدارة بوالص الشحن.
/// يعتمد فقط على IBillOfLadingService.
/// يتطلب توكن JWT صالحاً لجميع العمليات.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BillOfLadingsController : ControllerBase
{
    private readonly IBillOfLadingService _billOfLadingService;

    /// <summary>
    /// ينشئ نسخة جديدة من Controller البوالص.
    /// </summary>
    public BillOfLadingsController(IBillOfLadingService billOfLadingService)
    {
        _billOfLadingService = billOfLadingService;
    }

    /// <summary>
    /// يجلب صفحة من البوالص.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BillOfLadingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<BillOfLadingDto>>> GetAll(
        [FromQuery] BillOfLadingQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await _billOfLadingService.GetAllAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// يجلب بوليصة واحدة.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BillOfLadingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BillOfLadingDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var billOfLading = await _billOfLadingService.GetByIdAsync(id, cancellationToken);
        return Ok(billOfLading);
    }

    /// <summary>
    /// يجلب بوليصة البضاعة (sub-resource).
    /// </summary>
    [HttpGet("/api/packages/{packageId:int}/billoflading")]
    [ProducesResponseType(typeof(BillOfLadingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BillOfLadingDto>> GetByPackageId(
        int packageId,
        CancellationToken cancellationToken)
    {
        var billOfLading = await _billOfLadingService.GetByPackageIdAsync(packageId, cancellationToken);
        return Ok(billOfLading);
    }

    /// <summary>
    /// ينشئ بوليصة جديدة.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BillOfLadingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BillOfLadingDto>> Create(
        [FromBody] CreateBillOfLadingRequest request,
        CancellationToken cancellationToken)
    {
        var billOfLading = await _billOfLadingService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = billOfLading.Id }, billOfLading);
    }

    /// <summary>
    /// يحدّث بوليصة موجودة.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BillOfLadingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BillOfLadingDto>> Update(
        int id,
        [FromBody] UpdateBillOfLadingRequest request,
        CancellationToken cancellationToken)
    {
        var billOfLading = await _billOfLadingService.UpdateAsync(id, request, cancellationToken);
        return Ok(billOfLading);
    }

    /// <summary>
    /// يحذف بوليصة.
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
        await _billOfLadingService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}