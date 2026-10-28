using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShipFlow.Application.Common.Exceptions;
using ShipFlow.Application.Common.Interfaces;
using ShipFlow.Application.Common.Settings;
using ShipFlow.Application.Features.Auth.Dtos;
using ShipFlow.Domain.Entities;

namespace ShipFlow.Application.Features.Auth.Services;

/// <summary>
/// تنفيذ خدمة المصادقة.
/// يعتمد على IApplicationDbContext (بيانات)، IPasswordHasher (BCrypt)،
/// IJwtTokenGenerator (توليد التوكن)، وIOptions&lt;JwtSettings&gt; (لحساب ExpiresAt).
/// </summary>
public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// ينشئ نسخة جديدة من خدمة المصادقة.
    /// </summary>
    public AuthService(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// يسجّل مستخدماً جديداً ويعيد التوكن مع بيانات المستخدم.
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // 1) فحص تفرّد Username.
        var usernameExists = await _context.Users
            .AnyAsync(u => u.Username == request.Username, cancellationToken);

        if (usernameExists)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(RegisterRequest.Username),
                    "اسم المستخدم مستخدم بالفعل. اختر اسماً آخر.")
            });
        }

        // 2) فحص تفرّد Email.
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(RegisterRequest.Email),
                    "البريد الإلكتروني مستخدم بالفعل. اختر بريداً آخر.")
            });
        }

        // 3) إنشاء المستخدم (PasswordHash فقط — كلمة المرور لا تُخزَّن).
        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            FullName = request.FullName.Trim(),
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // 4) توليد التوكن وإرجاع الرد.
        return BuildAuthResponse(user);
    }

    /// <summary>
    /// يسجّل دخول المستخدم ويعيد التوكن مع بيانات المستخدم.
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // 1) جلب المستخدم بـUsername.
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        // 2) رسالة موحّدة (لا نكشف إن كان المستخدم موجوداً — حماية من enumeration).
        var invalidCredentialsFailure = new[]
        {
            new ValidationFailure(
                nameof(LoginRequest.Username),
                "بيانات الدخول غير صحيحة.")
        };

        if (user is null)
        {
            throw new ValidationException(invalidCredentialsFailure);
        }

        // 3) التحقق من كلمة المرور.
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new ValidationException(invalidCredentialsFailure);
        }

        // 4) فحص IsActive (معلومة منفصلة — لا نخلطها برسالة "بيانات خاطئة" لأن الحساب موجود).
        if (!user.IsActive)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(LoginRequest.Username),
                    "الحساب موقوف. تواصل مع المسؤول.")
            });
        }

        // 5) توليد التوكن وإرجاع الرد.
        return BuildAuthResponse(user);
    }

    /// <summary>
    /// يجلب بيانات المستخدم الحالي (بدون كلمة مرور).
    /// </summary>
    public async Task<UserDto> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            throw NotFoundException.ForEntity("User", userId);
        }

        return MapToUserDto(user);
    }

    /// <summary>
    /// يبني AuthResponse (token + expiresAt + user) من كيان المستخدم.
    /// </summary>
    private AuthResponse BuildAuthResponse(User user)
    {
        var token = _tokenGenerator.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = MapToUserDto(user)
        };
    }

    /// <summary>
    /// يحوّل كيان User إلى UserDto (بدون PasswordHash).
    /// </summary>
    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}