namespace ShipFlow.Domain.Enums;

/// <summary>
/// حالة البضاعة/الطرد خلال دورة الشحن.
/// </summary>
public enum PackageStatus
{
    /// <summary>تم الإدخال في المستودع، بانتظار التجميع.</summary>
    Received = 0,

    /// <summary>تم تجميعها في شحنة، بانتظار الانطلاق.</summary>
    AssignedToShipment = 1,

    /// <summary>في الطريق مع الشحنة.</summary>
    InTransit = 2,

    /// <summary>وصلت مستودع الوجهة.</summary>
    Arrived = 3,

    /// <summary>تم استلامها من قبل الزبون.</summary>
    Delivered = 4,

    /// <summary>تم إرجاعها (مرتجع).</summary>
    Returned = 5,

    /// <summary>ملغية (لا تُشحن).</summary>
    Cancelled = 6
}