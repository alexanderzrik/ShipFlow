namespace ShipFlow.Application.Features.Customers.Dtos;

/// <summary>
/// بيانات تحديث زبون موجود.
/// Id يُستلم من الـURL وليس من هذا الكائن.
/// </summary>
public class UpdateCustomerRequest
{
    /// <summary>الاسم الكامل (مطلوب، حتى 200 حرف).</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>رقم الهاتف الرئيسي (مطلوب، حتى 30 حرف).</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>رقم هاتف بديل (اختياري، حتى 30 حرف).</summary>
    public string? AlternativePhoneNumber { get; set; }

    /// <summary>العنوان (اختياري، حتى 500 حرف).</summary>
    public string? Address { get; set; }

    /// <summary>ملاحظات (اختياري، حتى 1000 حرف).</summary>
    public string? Notes { get; set; }

    /// <summary>هل الزبون نشط؟ (افتراضي: true).</summary>
    public bool IsActive { get; set; } = true;
}