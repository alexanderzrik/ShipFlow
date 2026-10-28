namespace ShipFlow.Application.Common.Settings;

/// <summary>
/// إعدادات JWT (تُقرأ من appsettings.json، قسم "Jwt").
/// تُسجَّل عبر IOptions<JwtSettings> في Program.cs.
/// </summary>
public class JwtSettings
{
    /// <summary>اسم القسم في appsettings.</summary>
    public const string SectionName = "Jwt";

    /// <summary>المفتاح السري لتوقيع التوكن (يجب أن يكون 32 حرفاً على الأقل، والموصى به 64+).</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>جهة إصدار التوكن (Issuer) — يُتحقَّق منها عند استقبال التوكن.</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>الجهة المستهدفة (Audience) — يُتحقَّق منها عند استقبال التوكن.</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>مدة صلاحية التوكن بالدقائق. الافتراضي: 60.</summary>
    public int ExpiryMinutes { get; set; } = 60;
}