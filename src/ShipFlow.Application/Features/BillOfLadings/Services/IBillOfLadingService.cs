using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.BillOfLadings.Dtos;
using ShipFlow.Application.Features.BillOfLadings.Queries;

namespace ShipFlow.Application.Features.BillOfLadings.Services;

/// <summary>
/// خدمة إدارة بوالص الشحن.
/// تعرّف العمليات المتاحة على كيانات البوليصة (قراءة، إنشاء، تعديل، حذف).
/// </summary>
public interface IBillOfLadingService
{
    /// <summary>
    /// يجلب صفحة من البوالص مع دعم الفلترة (PackageId)، البحث (BillNumber/IssuedBy)،
    /// والترتيب الديناميكي، مع تجزئة (Skip/Take).
    /// </summary>
    Task<PagedResult<BillOfLadingDto>> GetAllAsync(BillOfLadingQueryParameters query, CancellationToken cancellationToken = default);

    /// <summary>
    /// يجلب بوليصة واحدة بمعرّفها.
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد البوليصة.</exception>
    Task<BillOfLadingDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// يجلب بوليصة البضاعة المرتبطة (علاقة 1:1).
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">
    /// إذا لم تُوجد البضاعة أو لم تكن لها بوليصة بعد.
    /// </exception>
    Task<BillOfLadingDto> GetByPackageIdAsync(int packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ينشئ بوليصة جديدة:
    /// - يتحقق من وجود البضاعة (404 إن لم تُوجد).
    /// - يتحقق من عدم وجود بوليصة مسبقة للبضاعة (400 إن وُجدت — يطبّق Unique constraint بلطف).
    /// - يتحقق من تفرّد BillNumber إن مُرّر.
    /// - يولّد BillNumber تلقائياً إن لم يُمرَّر.
    /// - IssuedAt = UtcNow إن لم يُمرَّر (يسمح بالـbackdating عند تمريره).
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد البضاعة.</exception>
    /// <exception cref="FluentValidation.ValidationException">لأي انتهاك لقواعد العمل.</exception>
    Task<BillOfLadingDto> CreateAsync(CreateBillOfLadingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يحدّث بيانات بوليصة موجودة (IssuedBy, DeliveryFee, Notes فقط — ثوابت أخرى).
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد البوليصة.</exception>
    Task<BillOfLadingDto> UpdateAsync(int id, UpdateBillOfLadingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يحذف بوليصة بمعرّفها.
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد البوليصة.</exception>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}