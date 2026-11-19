using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.BillOfLadings.Dtos;

public class BillOfLadingDto
{
    public int Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string? IssuedBy { get; set; }
    public decimal DeliveryFee { get; set; }
    public Currency DeliveryFeeCurrency { get; set; } // الجديد
    public string? Notes { get; set; }
    
    public int PackageId { get; set; }
    public string PackageTrackingNumber { get; set; } = string.Empty;
    public string PackageDescription { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public int CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}