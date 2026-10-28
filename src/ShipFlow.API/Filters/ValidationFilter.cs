using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShipFlow.API.Filters;

/// <summary>
/// فلتر عمليات يعمل قبل كل Action في الـControllers.
/// يبحث عن أي <see cref="IValidator{T}"/> مطابق لنوع كل وسيط في الـAction،
/// ويشغّله إن وُجد. عند فشل أي قاعدة، يرمي <see cref="ValidationException"/>
/// ليتمكن الـ<see cref="Middleware.ExceptionHandlingMiddleware"/> من تحويله إلى 400
/// بصيغة ProblemDetails موحّدة (errors dictionary).
/// </summary>
/// <remarks>
/// لماذا لا نستخدم FluentValidation.AspNetCore؟
/// لأن الحزمة مهملة (Deprecated) رسمياً ولا تدعم .NET 8.
/// هذا الفلتر هو البديل الموصى به، ويضعنا نحن في تحكم كامل بشكل الاستجابة.
/// </remarks>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// ينشئ نسخة جديدة من الفلتر.
    /// </summary>
    /// <param name="serviceProvider">
    /// مزوّد خدمات DI الخاص بالطلب الحالي (Scoped).
    /// نستخدمه لطلب الـValidators ديناميكياً حسب نوع الوسيط.
    /// </param>
    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// يُنفَّذ قبل الـAction. إن وُجد Validator مطابق لأي وسيط وفشل التحقق،
    /// يرمي ValidationException. وإلا يمرّر الطلب للـAction.
    /// </summary>
    /// <param name="context">سياق تنفيذ الـAction (يحتوي على ActionArguments).</param>
    /// <param name="next">دالة تمرير الطلب إلى المرحلة التالية (الـAction نفسه).</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // نجمع كل الأخطاء من كل الـValidators قبل الرمي،
        // حتى يستلم المستخدم كل رسائل الخطأ دفعة واحدة.
        var allFailures = new List<ValidationFailure>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            // نبني النوع IValidator<TArgument> ديناميكياً.
            // مثال: argument = CreateCustomerRequest → نطلب IValidator<CreateCustomerRequest>.
            var argumentType = argument.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            // GetService يُرجع null إذا لم يكن النوع مسجّلاً (مثل int id، CancellationToken، إلخ).
            // GetRequiredService كان سيرمي استثناءً، لذا نستخدم GetService ونتخطى.
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator is null)
            {
                continue;
            }

            // نلفّ الوسيط في ValidationContext ونشغّل الـValidator.
            var validationContext = new ValidationContext<object>(argument);
            var validationResult = await validator.ValidateAsync(validationContext);

            allFailures.AddRange(validationResult.Errors);
        }

        if (allFailures.Count > 0)
        {
            // FluentValidation.ValidationException يحمل الأخطاء في خاصية Errors.
            // الـMiddleware سيتلقاها ويحوّلها إلى 400.
            throw new ValidationException(allFailures);
        }

        // لا أخطاء → نكمل للـAction.
        await next();
    }
}