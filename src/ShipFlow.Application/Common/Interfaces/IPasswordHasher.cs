namespace ShipFlow.Application.Common.Interfaces;

/// <summary>
/// واجهة تشفير كلمات المرور.
/// تُستخدم في طبقة Application دون معرفة تفاصيل التشفير (تلك في Infrastructure — BCrypt).
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// يحوّل كلمة المرور الأصلية إلى تجزئة (hash) آمنة للتخزين.
    /// </summary>
    /// <param name="password">كلمة المرور الأصلية.</param>
    /// <returns>تجزئة BCrypt (سلسلة تحتوي على الملح + الهاش).</returns>
    string Hash(string password);

    /// <summary>
    /// يتحقق من مطابقة كلمة مرور أصلية لتجزئة مخزّنة.
    /// </summary>
    /// <param name="password">كلمة المرور الأصلية.</param>
    /// <param name="hash">التجزئة المخزّنة.</param>
    /// <returns>true إذا تطابقت، false خلاف ذلك.</returns>
    bool Verify(string password, string hash);
}