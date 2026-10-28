using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.Packages.Dtos;
using ShipFlow.Application.Features.Shipments.Dtos;
using ShipFlow.Application.Features.Shipments.Queries;

namespace ShipFlow.Application.Features.Shipments.Services;

/// <summary>
/// خدمة إدارة الشحنات.
/// تعرّف العمليات المتاحة على كيانات الشحنة (CRUD + إدارة البضائع المحمّلة).
/// </summary>
public interface IShipmentService
{
    /// <summary>
    /// يجلب صفحة من الشحنات مع دعم الفلترة (Status, FromCity, ToCity)،
    /// البحث (ShipmentNumber/DriverName)، والترتيب الديناميكي، مع تجزئة.
    /// </summary>
    Task<PagedResult<ShipmentDto>> GetAllAsync(ShipmentQueryParameters query, CancellationToken cancellationToken = default);

    /// <summary>
    /// يجلب شحنة واحدة بمعرّفها (مع PackageCount).
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد الشحنة.</exception>
    Task<ShipmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ينشئ شحنة جديدة:
    /// - يتحقق من تفرّد ShipmentNumber إن مُرّر.
    /// - يولّد ShipmentNumber تلقائياً إن لم يُمرَّر.
    /// - Status يبدأ Loading دائماً.
    /// </summary>
    /// <exception cref="FluentValidation.ValidationException">إذا كان ShipmentNumber مستخدماً.</exception>
    Task<ShipmentDto> CreateAsync(CreateShipmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يحدّث بيانات شحنة موجودة:
    /// - تحديث الحقول العادية مسموح دائماً.
    /// - تغيير Status مسموح فقط للأمام (Loading → Departed → InTransit → Arrived → Closed).
    /// - DepartureDate/ArrivalDate تُملأ تلقائياً عند الوصول لحالة Departed/Arrived.
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد الشحنة.</exception>
    /// <exception cref="FluentValidation.ValidationException">إذا كان الانتقال غير مسموح.</exception>
    Task<ShipmentDto> UpdateAsync(int id, UpdateShipmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يحذف شحنة بمعرّفها.
    /// مسموح فقط إذا كانت Status=Loading ولا تحتوي بضائع.
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد الشحنة.</exception>
    /// <exception cref="FluentValidation.ValidationException">إذا كانت الحالة تمنع الحذف أو تحوي بضائع.</exception>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// يضيف مجموعة بضائع إلى شحنة (يُعيد الشحنة بعد التحديث مع PackageCount الجديد).
    /// مسموح فقط إذا كانت Status=Loading، وكل البضائع موجودة وغير معيَّنة لشحنة أخرى.
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد الشحنة.</exception>
    /// <exception cref="FluentValidation.ValidationException">لأي انتهاك لقواعد الإضافة.</exception>
    Task<ShipmentDto> AssignPackagesAsync(int shipmentId, AssignPackagesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// يزيل بضاعة من شحنة (يُعيد الشحنة بعد التحديث مع PackageCount الجديد).
    /// مسموح فقط إذا كانت Status=Loading، والبضاعة تنتمي فعلاً للشحنة.
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد الشحنة.</exception>
    /// <exception cref="FluentValidation.ValidationException">لأي انتهاك لقواعد الإزالة.</exception>
    Task<ShipmentDto> UnassignPackageAsync(int shipmentId, int packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// يجلب قائمة البضائع المحمّلة في شحنة (مرتبة بـTrackingNumber).
    /// </summary>
    /// <exception cref="Common.Exceptions.NotFoundException">إذا لم تُوجد الشحنة.</exception>
    Task<IReadOnlyList<PackageDto>> GetPackagesAsync(int shipmentId, CancellationToken cancellationToken = default);
}