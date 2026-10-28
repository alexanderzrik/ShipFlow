using System.Net;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Application.Common.Exceptions;

namespace ShipFlow.API.Middleware;

/// <summary>
/// Middleware مركزي لاعتراض الاستثناءات وتحويلها إلى ردود HTTP موحّدة.
/// يحوّل NotFoundException إلى 404، وValidationException إلى 400 مع قائمة الأخطاء،
/// وأي استثناء آخر إلى 500. يعيد الردود بصيغة ProblemDetails (معيار RFC 7807).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// ينشئ نسخة جديدة من الـmiddleware.
    /// </summary>
    /// <param name="next">الـmiddleware التالي في خط أنابيب التنفيذ.</param>
    /// <param name="logger">مُسجّل الأحداث (يُحقن عبر DI).</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// يعالج الـrequest، ويعترض أي استثناء ويحوّله لردّ HTTP مناسب.
    /// </summary>
    /// <param name="context">سياق HTTP للطلب الحالي.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException exception)
        {
            _logger.LogWarning(exception, "Entity not found: {Message}", exception.Message);
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.NotFound,
                "Not Found",
                exception.Message);
        }
        catch (FluentValidation.ValidationException exception)
        {
            _logger.LogWarning(exception, "Validation failed for {Path}", context.Request.Path);

            // تجميع الأخطاء حسب اسم الحقل:
            // { "FullName": ["الاسم الكامل مطلوب."],
            //   "PhoneNumber": ["رقم الهاتف يجب ألا يتجاوز 30 حرف."] }
            var errors = exception.Errors
                .GroupBy(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).ToArray());

            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.BadRequest,
                "Validation Failed",
                "واحد أو أكثر من الحقول غير صالح.",
                errors);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "حدث خطأ غير متوقع. الرجاء المحاولة لاحقاً.");
        }
    }

    /// <summary>
    /// يكتب رد ProblemDetails موحّد على الـresponse.
    /// </summary>
    /// <param name="context">سياق HTTP.</param>
    /// <param name="statusCode">رمز الحالة.</param>
    /// <param name="title">عنوان قصير للخطأ.</param>
    /// <param name="detail">تفصيل الرسالة.</param>
    /// <param name="errors">قاموس أخطاء اختياري (اسم الحقل ← الرسائل). يُضاف إلى Extensions.</param>
    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail,
        IDictionary<string, string[]>? errors = null)
    {
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors is not null)
        {
            // Extensions هي IDictionary<string, object?> تُسلسَل مع بقية الـProblemDetails
            // كحقل "errors" في الجذر — نفس صيغة ValidationProblemDetails الرسمية.
            problemDetails.Extensions["errors"] = errors;
        }

        await context.Response.WriteAsJsonAsync(
            problemDetails,
            options: null,
            contentType: "application/problem+json");
    }
}