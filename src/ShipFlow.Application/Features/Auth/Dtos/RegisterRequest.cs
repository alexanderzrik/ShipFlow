namespace ShipFlow.Application.Features.Auth.Dtos;

/// <summary>
/// بيانات طلب تسجيل مستخدم جديد.
/// يُرسَل عبر body في POST /api/auth/register.
/// </summary>
public class RegisterRequest
{
    /// <summary>اسم المستخدم (فريد، 3-50 حرف).</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>البريد الإلكتروني (فريد، صيغة بريد صحيحة، حتى 200 حرف).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>كلمة المرور (8-100 حرف، يجب أن تحتوي حرفاً ورقماً على الأقل).</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>الاسم الكامل (حتى 200 حرف).</summary>
    public string FullName { get; set; } = string.Empty;
}