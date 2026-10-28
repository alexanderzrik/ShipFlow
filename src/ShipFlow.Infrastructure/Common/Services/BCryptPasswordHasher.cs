using ShipFlow.Application.Common.Interfaces;

namespace ShipFlow.Infrastructure.Common.Services;

/// <summary>
/// تنفيذ تشفير كلمات المرور باستخدام BCrypt.
/// BCrypt يتضمّن الملح (salt) تلقائياً داخل الـhash،
/// لذا لا نحتاج إلى تخزين الملح منفصلاً.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    /// <summary>
    /// يشفّر كلمة المرور باستخدام BCrypt (work factor افتراضي = 11).
    /// </summary>
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// يتحقق من مطابقة كلمة المرور للتجزئة المخزّنة.
    /// </summary>
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}