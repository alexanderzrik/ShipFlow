using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.BillOfLadings.Dtos;

public class CreateBillOfLadingRequest
{
    public string? BillNumber { get; set; }
    public DateTime? IssuedAt { get; set; }
    public string? IssuedBy { get; set; }
    public decimal DeliveryFee { get; set; }
    public Currency DeliveryFeeCurrency { get; set; } = Currency.SYP; // الجديد
    public string? Notes { get; set; }
    public int PackageId { get; set; }
}