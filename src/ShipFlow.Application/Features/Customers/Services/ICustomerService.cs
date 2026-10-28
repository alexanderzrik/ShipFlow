using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.Customers.Dtos;
using ShipFlow.Application.Features.Customers.Queries;

namespace ShipFlow.Application.Features.Customers.Services;

/// <summary>
/// خدمة إدارة الزبائن.
/// تعرّف العمليات المتاحة على كيانات الزبون (قراءة، إنشاء، تعديل، حذف).
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// يجلب زبوناً واحداً بمعرّفه.
    /// </summary>
    /// <param name="id">معرّف الزبون.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>بيانات الزبون.</returns>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم يُوجد الزبون.</exception>
    Task<CustomerDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// يجلب صفحة من الزبائن مع دعم الفلترة والبحث والترتيب.
    /// </summary>
    /// <param name="query">معاملات الاستعلام (رقم الصفحة، الحجم، البحث، الترتيب، فلترة التنشيط).</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>نتيجة مُجزَّأة تحتوي العناصر + معلومات الصفحة (totalCount, totalPages, hasNext...).</returns>
    Task<PagedResult<CustomerDto>> GetAllAsync(CustomerQueryParameters query, CancellationToken cancellationToken = default);

    /// <summary>
    /// ينشئ زبوناً جديداً.
    /// </summary>
    /// <param name="request">بيانات الزبون الجديد.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>الزبون المُنشأ بمعرّفه الجديد.</returns>
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يحدّث بيانات زبون موجود.
    /// </summary>
    /// <param name="id">معرّف الزبون.</param>
    /// <param name="request">البيانات الجديدة.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>الزبون بعد التحديث.</returns>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم يُوجد الزبون.</exception>
    Task<CustomerDto> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يحذف زبوناً بمعرّفه.
    /// </summary>
    /// <param name="id">معرّف الزبون.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم يُوجد الزبون.</exception>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}