using ShipFlow.Domain.Enums;

namespace ShipFlow.Domain.Entities;

/// <summary>
/// يمثل بوليصة شحن — مستند قانوني يصدر لكل بضاعة.
/// </summary>
public class BillOfLading
{
    public int Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public string? IssuedBy { get; set; }

    /// <summary>أجرة التوصيل.</summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>عملة أجرة التوصيل (الافتراضي: ليرة سورية).</summary>
    public Currency DeliveryFeeCurrency { get; set; } = Currency.SYP;

    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign Key
    public int PackageId { get; set; }

    // Navigation Property
    public Package Package { get; set; } = null!;
}