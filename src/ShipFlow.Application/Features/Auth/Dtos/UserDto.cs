namespace ShipFlow.Application.Features.Auth.Dtos;

/// <summary>
/// بيانات المستخدم التي تُرسَل للعميل عند القراءة.
/// ⚠️ لا يحتوي PasswordHash أبداً — لا نُسرّب بيانات حساسة.
/// </summary>
public class UserDto
{
    /// <summary>المعرف الفريد للمستخدم.</summary>
    public int Id { get; set; }

    /// <summary>اسم المستخدم.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>البريد الإلكتروني.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>الاسم الكامل.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>دور المستخدم (Admin / User / ...).</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>هل الحساب نشط؟</summary>
    public bool IsActive { get; set; }

    /// <summary>تاريخ إنشاء السجل (UTC).</summary>
    public DateTime CreatedAt { get; set; }
}