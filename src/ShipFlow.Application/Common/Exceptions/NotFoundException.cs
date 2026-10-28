namespace ShipFlow.Application.Common.Exceptions;

/// <summary>
/// استثناء يُرمى عند عدم العثور على كيان مطلوب.
/// يُستخدم للتمييز بين "عنصر غير موجود" (404) وأخطاء أخرى (500).
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// ينشئ نسخة جديدة من الاستثناء.
    /// </summary>
    /// <param name="message">رسالة توضح العنصر المفقود.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// ينشئ استثناءً موحّداً لكيان غير موجود.
    /// </summary>
    /// <param name="entityName">اسم الكيان (مثل "Customer").</param>
    /// <param name="key">المفتاح الذي تم البحث به (مثل 5).</param>
    /// <returns>نسخة جديدة من NotFoundException برسالة موحّدة.</returns>
    public static NotFoundException ForEntity(string entityName, object key)
    {
        return new NotFoundException($"Entity \"{entityName}\" with key ({key}) was not found.");
    }
}