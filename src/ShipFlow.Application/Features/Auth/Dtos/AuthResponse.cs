namespace ShipFlow.Application.Features.Auth.Dtos;

/// <summary>
/// الرد الموحّد بعد تسجيل الدخول أو التسجيل.
/// يحتوي التوكن + وقت انتهاء صلاحيته + بيانات المستخدم — كل ما يحتاجه العميل.
/// </summary>
public class AuthResponse
{
    /// <summary>توكن JWT الموقّع.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>وقت انتهاء صلاحية التوكن (UTC).</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>بيانات المستخدم (بدون أي حقول حساسة).</summary>
    public UserDto User { get; set; } = new();
}