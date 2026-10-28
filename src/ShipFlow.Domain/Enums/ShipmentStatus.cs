namespace ShipFlow.Domain.Enums;

/// <summary>
/// حالة الشحنة (الرحلة/السيارة).
/// </summary>
public enum ShipmentStatus
{
    /// <summary>قيد التجميع — يتم إضافة بضائع.</summary>
    Loading = 0,

    /// <summary>انطلقت من المصدر.</summary>
    Departed = 1,

    /// <summary>في الطريق بين المدن.</summary>
    InTransit = 2,

    /// <summary>وصلت الوجهة.</summary>
    Arrived = 3,

    /// <summary>أُغلقت — تم تفريغ البضائع.</summary>
    Closed = 4,

    /// <summary>ملغية.</summary>
    Cancelled = 5
}