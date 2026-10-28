namespace ShipFlow.Domain.Entities;

/// <summary>
/// يمثل مستخدم النظام (موظف الشركة) الذي يسجّل الدخول للـAPI.
/// منفصل تماماً عن Customer (الزبون) — Users هم مشغّلو النظام، Customers بيانات.
/// </summary>
public class User
{
    /// <summary>المعرف الفريد للمستخدم.</summary>
    public int Id { get; set; }

    /// <summary>اسم المستخدم (فريد، يُستخدم لتسجيل الدخول).</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>البريد الإلكتروني (فريد).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>تجزئة كلمة المرور (BCrypt hash — لا تُخزَّن كلمة المرور الأصلية أبداً).</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>الاسم الكامل للمستخدم.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>دور المستخدم (Admin / User / ...). الافتراضي: User.</summary>
    public string Role { get; set; } = "User";

    /// <summary>هل الحساب نشط؟ (false = موقوف).</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>تاريخ إنشاء السجل (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>تاريخ آخر تحديث (UTC) — null إذا لم يُحدَّث.</summary>
    public DateTime? UpdatedAt { get; set; }
}