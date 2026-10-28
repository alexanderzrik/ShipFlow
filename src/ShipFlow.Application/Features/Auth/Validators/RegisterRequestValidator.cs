using FluentValidation;
using ShipFlow.Application.Features.Auth.Dtos;

namespace ShipFlow.Application.Features.Auth.Validators;

/// <summary>
/// قواعد التحقق من صحة بيانات تسجيل مستخدم جديد.
/// يفرض سياسة كلمة مرور قوية (8+ أحرف، حرف + رقم) لحماية الحسابات.
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    /// <summary>
    /// يُهيّئ قواعد التحقق لكل حقل في <see cref="RegisterRequest"/>.
    /// </summary>
    public RegisterRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("اسم المستخدم مطلوب.")
            .MinimumLength(3).WithMessage("اسم المستخدم يجب أن يكون 3 أحرف على الأقل.")
            .MaximumLength(50).WithMessage("اسم المستخدم يجب ألا يتجاوز 50 حرف.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.")
            .MaximumLength(200).WithMessage("البريد الإلكتروني يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("كلمة المرور مطلوبة.")
            .MinimumLength(8).WithMessage("كلمة المرور يجب أن تكون 8 أحرف على الأقل.")
            .MaximumLength(100).WithMessage("كلمة المرور يجب ألا تتجاوز 100 حرف.")
            .Matches("[A-Za-z]").WithMessage("كلمة المرور يجب أن تحتوي على حرف واحد على الأقل.")
            .Matches("[0-9]").WithMessage("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب.")
            .MaximumLength(200).WithMessage("الاسم الكامل يجب ألا يتجاوز 200 حرف.");
    }
}