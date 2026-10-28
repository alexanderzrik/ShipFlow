using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using ShipFlow.Application.Common.Exceptions;
using ShipFlow.Application.Common.Interfaces;
using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.Packages.Dtos;
using ShipFlow.Application.Features.Shipments.Dtos;
using ShipFlow.Application.Features.Shipments.Queries;
using ShipFlow.Domain.Entities;
using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.Shipments.Services;

/// <summary>
/// تنفيذ خدمة إدارة الشحنات.
/// يحتوي منطق العمل الحسّاس:
/// - فحص تفرّد ShipmentNumber.
/// - توليد ShipmentNumber تلقائياً.
/// - حالة الشحنة "للأمام فقط" + إدارة تلقائية لتواريخ الانطلاق/الوصول.
/// - إدارة البضائع المحمّلة (إضافة/إزالة) بقيود Status=Loading.
/// </summary>
public class ShipmentService : IShipmentService
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// ينشئ نسخة جديدة من خدمة الشحنات.
    /// </summary>
    public ShipmentService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// يجلب صفحة من الشحنات مع دعم الفلترة، البحث، الترتيب، والتجزئة.
    /// </summary>
    public async Task<PagedResult<ShipmentDto>> GetAllAsync(ShipmentQueryParameters query, CancellationToken cancellationToken = default)
    {
        var queryable = _context.Shipments.AsNoTracking();

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(s => s.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.FromCity))
        {
            var term = query.FromCity.Trim();
            queryable = queryable.Where(s => s.FromCity.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.ToCity))
        {
            var term = query.ToCity.Trim();
            queryable = queryable.Where(s => s.ToCity.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            queryable = queryable.Where(s =>
                s.ShipmentNumber.Contains(term) ||
                (s.DriverName != null && s.DriverName.Contains(term)));
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = ApplySorting(queryable, query.SortBy, query.SortOrder);

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new ShipmentDto
            {
                Id = s.Id,
                ShipmentNumber = s.ShipmentNumber,
                FromCity = s.FromCity,
                ToCity = s.ToCity,
                DriverName = s.DriverName,
                DriverPhone = s.DriverPhone,
                VehicleNumber = s.VehicleNumber,
                Status = s.Status,
                DepartureDate = s.DepartureDate,
                ArrivalDate = s.ArrivalDate,
                Notes = s.Notes,
                PackageCount = s.Packages.Count,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ShipmentDto>(items, totalCount, query.Page, query.PageSize);
    }

    /// <summary>
    /// يجلب شحنة واحدة بمعرّفها (مع PackageCount).
    /// </summary>
    public async Task<ShipmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dto = await _context.Shipments
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new ShipmentDto
            {
                Id = s.Id,
                ShipmentNumber = s.ShipmentNumber,
                FromCity = s.FromCity,
                ToCity = s.ToCity,
                DriverName = s.DriverName,
                DriverPhone = s.DriverPhone,
                VehicleNumber = s.VehicleNumber,
                Status = s.Status,
                DepartureDate = s.DepartureDate,
                ArrivalDate = s.ArrivalDate,
                Notes = s.Notes,
                PackageCount = s.Packages.Count,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            throw NotFoundException.ForEntity("Shipment", id);
        }

        return dto;
    }

    /// <summary>
    /// ينشئ شحنة جديدة. Status يبدأ Loading. ShipmentNumber يُفحص أو يُولَّد.
    /// </summary>
    public async Task<ShipmentDto> CreateAsync(CreateShipmentRequest request, CancellationToken cancellationToken = default)
    {
        string shipmentNumber;

        if (!string.IsNullOrWhiteSpace(request.ShipmentNumber))
        {
            shipmentNumber = request.ShipmentNumber.Trim();

            var isDuplicate = await _context.Shipments
                .AnyAsync(s => s.ShipmentNumber == shipmentNumber, cancellationToken);

            if (isDuplicate)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(
                        nameof(CreateShipmentRequest.ShipmentNumber),
                        "رقم الشحنة مستخدم بالفعل. اختر رقماً آخر أو اتركه فارغاً للتوليد التلقائي.")
                });
            }
        }
        else
        {
            shipmentNumber = GenerateShipmentNumber();
        }

        var shipment = new Shipment
        {
            ShipmentNumber = shipmentNumber,
            FromCity = request.FromCity,
            ToCity = request.ToCity,
            DriverName = request.DriverName,
            DriverPhone = request.DriverPhone,
            VehicleNumber = request.VehicleNumber,
            Status = ShipmentStatus.Loading,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(shipment, 0);
    }

    /// <summary>
    /// يحدّث بيانات شحنة (مع فحص "Status للأمام" وإدارة تلقائية للتواريخ).
    /// </summary>
    public async Task<ShipmentDto> UpdateAsync(int id, UpdateShipmentRequest request, CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (shipment is null)
        {
            throw NotFoundException.ForEntity("Shipment", id);
        }

        shipment.FromCity = request.FromCity;
        shipment.ToCity = request.ToCity;
        shipment.DriverName = request.DriverName;
        shipment.DriverPhone = request.DriverPhone;
        shipment.VehicleNumber = request.VehicleNumber;
        shipment.Notes = request.Notes;
        shipment.UpdatedAt = DateTime.UtcNow;

        if (request.Status.HasValue && request.Status.Value != shipment.Status)
        {
            if (!IsForwardTransition(shipment.Status, request.Status.Value))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(
                        nameof(UpdateShipmentRequest.Status),
                        $"لا يمكن تغيير الحالة من '{shipment.Status}' إلى '{request.Status.Value}'. " +
                        "الانتقالات المسموحة: Loading → Departed → InTransit → Arrived → Closed (للأمام فقط).")
                });
            }

            shipment.Status = request.Status.Value;

            if (shipment.Status >= ShipmentStatus.Departed && shipment.DepartureDate is null)
            {
                shipment.DepartureDate = DateTime.UtcNow;
            }

            if (shipment.Status >= ShipmentStatus.Arrived && shipment.ArrivalDate is null)
            {
                shipment.ArrivalDate = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var packageCount = await _context.Packages
            .CountAsync(p => p.ShipmentId == shipment.Id, cancellationToken);

        return MapToDto(shipment, packageCount);
    }

    /// <summary>
    /// يحذف شحنة (فقط إذا Status=Loading ولا تحوي بضائع).
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (shipment is null)
        {
            throw NotFoundException.ForEntity("Shipment", id);
        }

        if (shipment.Status != ShipmentStatus.Loading)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    "Status",
                    $"لا يمكن حذف شحنة في حالة '{shipment.Status}'. الحذف مسموح فقط في حالة Loading.")
            });
        }

        var packageCount = await _context.Packages
            .CountAsync(p => p.ShipmentId == id, cancellationToken);

        if (packageCount > 0)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    "Packages",
                    $"لا يمكن حذف الشحنة — تحتوي على {packageCount} بضاعة. أزل البضائع أولاً.")
            });
        }

        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// يضيف بضائع إلى شحنة (فقط إذا Status=Loading، وكل البضائع موجودة وغير معيَّنة).
    /// </summary>
    /// <remarks>
    /// ملاحظة تقنية: لا نستخدم .Contains(p.Id) على List&lt;int&gt;
    /// لأن EF Core 8 يترجمها إلى OPENJSON، وSQL Server LocalDB v11 (2012)
    /// لا يدعم OPENJSON (أُضيف في 2016). نستخدم حلقة FirstOrDefaultAsync
    /// لكل معرّف — PK lookup سريع (~1ms) وآمن مع أي إصدار SQL.
    /// </remarks>
    public async Task<ShipmentDto> AssignPackagesAsync(int shipmentId, AssignPackagesRequest request, CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(s => s.Id == shipmentId, cancellationToken);

        if (shipment is null)
        {
            throw NotFoundException.ForEntity("Shipment", shipmentId);
        }

        if (shipment.Status != ShipmentStatus.Loading)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    "Status",
                    $"لا يمكن إضافة بضائع لشحنة في حالة '{shipment.Status}'. الإضافة مسموحة فقط في حالة Loading.")
            });
        }

        // تحميل البضائع واحدة واحدة (بدل Contains → OPENJSON).
        var packages = new List<Package>();
        foreach (var packageId in request.PackageIds)
        {
            var package = await _context.Packages
                .FirstOrDefaultAsync(p => p.Id == packageId, cancellationToken);

            if (package is not null)
            {
                packages.Add(package);
            }
        }

        if (packages.Count != request.PackageIds.Count)
        {
            var foundIds = packages.Select(p => p.Id).ToHashSet();
            var missingIds = request.PackageIds.Where(pid => !foundIds.Contains(pid)).ToList();
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(AssignPackagesRequest.PackageIds),
                    $"البضائع التالية غير موجودة: {string.Join(", ", missingIds)}")
            });
        }

        var alreadyInThisShipment = packages
            .Where(p => p.ShipmentId == shipmentId)
            .ToList();

        if (alreadyInThisShipment.Count > 0)
        {
            var ids = alreadyInThisShipment.Select(p => p.Id).ToList();
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(AssignPackagesRequest.PackageIds),
                    $"البضائع التالية معيَّنة مسبقاً لهذه الشحنة: {string.Join(", ", ids)}")
            });
        }

        var assignedElsewhere = packages
            .Where(p => p.ShipmentId.HasValue && p.ShipmentId.Value != shipmentId)
            .ToList();

        if (assignedElsewhere.Count > 0)
        {
            var ids = assignedElsewhere.Select(p => p.Id).ToList();
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(AssignPackagesRequest.PackageIds),
                    $"البضائع التالية معيَّنة مسبقاً لشحنة أخرى: {string.Join(", ", ids)}")
            });
        }

        foreach (var package in packages)
        {
            package.ShipmentId = shipmentId;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var newCount = await _context.Packages
            .CountAsync(p => p.ShipmentId == shipmentId, cancellationToken);

        return MapToDto(shipment, newCount);
    }

    /// <summary>
    /// يزيل بضاعة من شحنة (فقط إذا Status=Loading، والبضاعة تنتمي فعلاً للشحنة).
    /// </summary>
    public async Task<ShipmentDto> UnassignPackageAsync(int shipmentId, int packageId, CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(s => s.Id == shipmentId, cancellationToken);

        if (shipment is null)
        {
            throw NotFoundException.ForEntity("Shipment", shipmentId);
        }

        if (shipment.Status != ShipmentStatus.Loading)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    "Status",
                    $"لا يمكن إزالة بضائع من شحنة في حالة '{shipment.Status}'. الإزالة مسموحة فقط في حالة Loading.")
            });
        }

        var package = await _context.Packages
            .FirstOrDefaultAsync(p => p.Id == packageId, cancellationToken);

        if (package is null)
        {
            throw NotFoundException.ForEntity("Package", packageId);
        }

        if (package.ShipmentId != shipmentId)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    "PackageId",
                    $"البضاعة {packageId} ليست جزءاً من الشحنة {shipmentId}.")
            });
        }

        package.ShipmentId = null;
        await _context.SaveChangesAsync(cancellationToken);

        var newCount = await _context.Packages
            .CountAsync(p => p.ShipmentId == shipmentId, cancellationToken);

        return MapToDto(shipment, newCount);
    }

    /// <summary>
    /// يجلب قائمة البضائع المحمّلة في شحنة (مرتبة بـTrackingNumber).
    /// </summary>
    public async Task<IReadOnlyList<PackageDto>> GetPackagesAsync(int shipmentId, CancellationToken cancellationToken = default)
    {
        var shipmentExists = await _context.Shipments
            .AnyAsync(s => s.Id == shipmentId, cancellationToken);

        if (!shipmentExists)
        {
            throw NotFoundException.ForEntity("Shipment", shipmentId);
        }

        return await _context.Packages
            .AsNoTracking()
            .Where(p => p.ShipmentId == shipmentId)
            .OrderBy(p => p.TrackingNumber)
            .Select(p => new PackageDto
            {
                Id = p.Id,
                TrackingNumber = p.TrackingNumber,
                Description = p.Description,
                Weight = p.Weight,
                Status = p.Status,
                ReceiverName = p.ReceiverName,
                ReceiverPhone = p.ReceiverPhone,
                DestinationCity = p.DestinationCity,
                Notes = p.Notes,
                CustomerId = p.CustomerId,
                CustomerName = p.Customer.FullName,
                ShipmentId = p.ShipmentId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// يفحص إذا كان الانتقال من <paramref name="current"/> إلى <paramref name="target"/> مسموحاً.
    /// الانتقالات المسموحة: Loading(0) → Departed(1) → InTransit(2) → Arrived(3) → Closed(4).
    /// Cancelled(5) لا يُسمح به عبر Update (له Endpoint منفصل).
    /// </summary>
    private static bool IsForwardTransition(ShipmentStatus current, ShipmentStatus target)
    {
        if (target == ShipmentStatus.Cancelled)
        {
            return false;
        }

        if (current == ShipmentStatus.Closed || current == ShipmentStatus.Cancelled)
        {
            return false;
        }

        return (int)target > (int)current;
    }

    /// <summary>
    /// يحوّل كيان Shipment إلى ShipmentDto.
    /// </summary>
    private static ShipmentDto MapToDto(Shipment shipment, int packageCount)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,
            ShipmentNumber = shipment.ShipmentNumber,
            FromCity = shipment.FromCity,
            ToCity = shipment.ToCity,
            DriverName = shipment.DriverName,
            DriverPhone = shipment.DriverPhone,
            VehicleNumber = shipment.VehicleNumber,
            Status = shipment.Status,
            DepartureDate = shipment.DepartureDate,
            ArrivalDate = shipment.ArrivalDate,
            Notes = shipment.Notes,
            PackageCount = packageCount,
            CreatedAt = shipment.CreatedAt,
            UpdatedAt = shipment.UpdatedAt
        };
    }

    /// <summary>
    /// يولّد رقم شحنة فريداً بصيغة SHN-yyyyMMddHHmmss-XXXX.
    /// </summary>
    private static string GenerateShipmentNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();
        return $"SHN-{timestamp}-{random}";
    }

    /// <summary>
    /// يطبّق الترتيب على IQueryable حسب اسم الحقل واتجاه الترتيب.
    /// </summary>
    private static IQueryable<Shipment> ApplySorting(
        IQueryable<Shipment> queryable,
        string sortBy,
        string sortOrder)
    {
        var descending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        var normalizedSortBy = (sortBy ?? string.Empty).ToLowerInvariant();

        return normalizedSortBy switch
        {
            "shipmentnumber" => descending
                ? queryable.OrderByDescending(s => s.ShipmentNumber)
                : queryable.OrderBy(s => s.ShipmentNumber),

            "fromcity" => descending
                ? queryable.OrderByDescending(s => s.FromCity)
                : queryable.OrderBy(s => s.FromCity),

            "tocity" => descending
                ? queryable.OrderByDescending(s => s.ToCity)
                : queryable.OrderBy(s => s.ToCity),

            "status" => descending
                ? queryable.OrderByDescending(s => s.Status)
                : queryable.OrderBy(s => s.Status),

            "departuredate" => descending
                ? queryable.OrderByDescending(s => s.DepartureDate)
                : queryable.OrderBy(s => s.DepartureDate),

            "arrivaldate" => descending
                ? queryable.OrderByDescending(s => s.ArrivalDate)
                : queryable.OrderBy(s => s.ArrivalDate),

            "updatedat" => descending
                ? queryable.OrderByDescending(s => s.UpdatedAt)
                : queryable.OrderBy(s => s.UpdatedAt),

            _ => descending
                ? queryable.OrderByDescending(s => s.CreatedAt)
                : queryable.OrderBy(s => s.CreatedAt)
        };
    }
}