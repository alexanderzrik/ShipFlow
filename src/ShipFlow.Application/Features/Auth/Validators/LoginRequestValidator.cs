using FluentValidation;
using ShipFlow.Application.Features.Auth.Dtos;

namespace ShipFlow.Application.Features.Auth.Validators;

/// <summary>
/// قواعد التحقق من صحة بيانات تسجيل الدخول.
/// بسيطة: فقط وجود اسم المستخدم وكلمة المرور — لا نفرض سياسة كلمة المرور هنا،
/// لأن المستخدم قد يمتلك حساباً قديماً بقواعد مختلفة.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>
    /// يُهيّئ قواعد التحقق لكل حقل في <see cref="LoginRequest"/>.
    /// </summary>
    public LoginRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("اسم المستخدم مطلوب.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("كلمة المرور مطلوبة.");
    }
}