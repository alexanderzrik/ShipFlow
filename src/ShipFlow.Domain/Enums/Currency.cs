namespace ShipFlow.Domain.Enums;

/// <summary>
/// العملات المدعومة في النظام.
/// القيمة 0 (SYP) هي الأساس، وبقية العملات تُساوي قيمتها بالليرة السورية.
/// </summary>
public enum Currency
{
    /// <summary>الليرة السورية (العملة الأساسية).</summary>
    SYP = 0,

    /// <summary>الدولار الأمريكي.</summary>
    USD = 1,

    /// <summary>اليورو.</summary>
    EUR = 2,

    /// <summary>الليرة التركية.</summary>
    TRY = 3
}