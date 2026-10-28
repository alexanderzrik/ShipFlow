using ShipFlow.Domain.Entities;

namespace ShipFlow.Application.Common.Interfaces;

/// <summary>
/// واجهة مولّد توكن JWT.
/// تُستخدم في طبقة Application لتوليد التوكن دون معرفة تفاصيل JWT (تلك في Infrastructure).
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// يولّد توكن JWT صالحاً لمستخدم معيّن، يحتوي على:
    /// معرف المستخدم، اسم المستخدم، البريد، الدور، ومعرّف فريد للتوكن (jti).
    /// </summary>
    /// <param name="user">المستخدم الذي سيُصدر له التوكن.</param>
    /// <returns>سلسلة JWT موقّعة وجاهزة للإرسال للعميل.</returns>
    string GenerateToken(User user);
}