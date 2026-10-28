using ShipFlow.Application.Features.Auth.Dtos;

namespace ShipFlow.Application.Features.Auth.Services;

/// <summary>
/// خدمة المصادقة.
/// تعرّف عمليات تسجيل المستخدمين، تسجيل الدخول، واسترجاع المستخدم الحالي.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// يسجّل مستخدماً جديداً:
    /// - يتحقق من تفرّد Username و Email → 400 إن كانا مستخدمَيْن.
    /// - يشفّر كلمة المرور عبر BCrypt.
    /// - ينشئ المستخدم بـRole="User", IsActive=true.
    /// - يولّد JWT ويعيده مع بيانات المستخدم.
    /// </summary>
    /// <exception cref="FluentValidation.ValidationException">
    /// إذا كان Username أو Email مستخدماً بالفعل.
    /// </exception>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يسجّل دخول مستخدم:
    /// - يبحث بـUsername.
    /// - يتحقق من كلمة المرور (BCrypt).
    /// - يفحص IsActive.
    /// - يولّد JWT ويعيده مع بيانات المستخدم.
    /// </summary>
    /// <exception cref="FluentValidation.ValidationException">
    /// إذا كانت بيانات الدخول خاطئة أو الحساب موقوف.
    /// (رسالة موحّدة "بيانات دخول غير صحيحة" — لا نكشف إن كان المستخدم موجوداً).
    /// </exception>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يجلب بيانات المستخدم الحالي (لـGET /api/auth/me).
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم يُوجد المستخدم.</exception>
    Task<UserDto> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
}