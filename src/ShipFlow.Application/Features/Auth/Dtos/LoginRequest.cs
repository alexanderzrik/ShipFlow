namespace ShipFlow.Application.Features.Auth.Dtos;

/// <summary>
/// بيانات طلب تسجيل الدخول.
/// يُرسَل عبر body في POST /api/auth/login.
/// </summary>
public class LoginRequest
{
    /// <summary>اسم المستخدم.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>كلمة المرور.</summary>
    public string Password { get; set; } = string.Empty;
}