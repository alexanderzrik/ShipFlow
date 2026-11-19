using ShipFlow.Domain.Enums;

namespace ShipFlow.Domain.Entities;

/// <summary>
/// يمثل بضاعة/طرد واحد يُشحن من زبون إلى وجهة.
/// </summary>
public class Package
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public PackageStatus Status { get; set; } = PackageStatus.Received;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public string? Notes { get; set; }

    /// <summary>أجرة التوصيل (تُضاف عند تسليم البضاعة للزبون).</summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>عملة أجرة التوصيل (الافتراضي: ليرة سورية).</summary>
    public Currency DeliveryFeeCurrency { get; set; } = Currency.SYP;

    /// <summary>تاريخ ووقت تسليم البضاعة للزبون (null إذا لم تُسلَّم بعد).</summary>
    public DateTime? DeliveredAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign Keys
    public int CustomerId { get; set; }
    public int? ShipmentId { get; set; }

    // Navigation Properties
    public Customer Customer { get; set; } = null!;
    public Shipment? Shipment { get; set; }
    public BillOfLading? BillOfLading { get; set; }
}