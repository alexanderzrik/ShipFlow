using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Application.Features.Auth.Dtos;
using ShipFlow.Application.Features.Auth.Services;

namespace ShipFlow.API.Controllers;

/// <summary>
/// نقاط نهاية المصادقة (تسجيل، دخول، بيانات المستخدم الحالي).
/// يعتمد فقط على IAuthService (لا يعرف EF Core ولا BCrypt).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// ينشئ نسخة جديدة من Controller المصادقة.
    /// </summary>
    /// <param name="authService">خدمة المصادقة (تُحقن عبر DI).</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// يسجّل مستخدماً جديداً ويعيد التوكن مع بيانات المستخدم.
    /// </summary>
    /// <param name="request">بيانات التسجيل.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>AuthResponse (token + expiresAt + user).</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// يسجّل دخول مستخدم ويعيد التوكن مع بيانات المستخدم.
    /// </summary>
    /// <param name="request">بيانات الدخول.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>AuthResponse (token + expiresAt + user).</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// يجلب بيانات المستخدم الحالي (من التوكن).
    /// يتطلب توكن JWT صالحاً في رأس Authorization.
    /// </summary>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>بيانات المستخدم (بدون PasswordHash).</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Me(CancellationToken cancellationToken)
    {
        // JWT middleware يملأ User (ClaimsPrincipal) من التوكن.
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            // لا يفترض أن يحدث (التوكن صادر من عندنا)، لكن احتياط للأمان.
            return Unauthorized();
        }

        var user = await _authService.GetCurrentUserAsync(userId, cancellationToken);
        return Ok(user);
    }
}