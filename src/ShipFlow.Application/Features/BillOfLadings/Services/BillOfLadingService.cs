using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using ShipFlow.Application.Common.Exceptions;
using ShipFlow.Application.Common.Interfaces;
using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.BillOfLadings.Dtos;
using ShipFlow.Application.Features.BillOfLadings.Queries;
using ShipFlow.Domain.Entities;

namespace ShipFlow.Application.Features.BillOfLadings.Services;

/// <summary>
/// تنفيذ خدمة إدارة بوالص الشحن.
/// يحتوي منطق العمل الحسّاس:
/// - فحص وجود البضاعة (FK → 404).
/// - فحص عدم وجود بوليصة مسبقة للبضاعة (Unique PackageId → 400 مفهوم).
/// - فحص تفرّد BillNumber.
/// - توليد BillNumber تلقائياً.
/// - إدارة IssuedAt (null → UtcNow، يسمح بالـbackdating).
/// </summary>
public class BillOfLadingService : IBillOfLadingService
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// ينشئ نسخة جديدة من خدمة بوالص الشحن.
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات (يُحقن عبر DI).</param>
    public BillOfLadingService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// يجلب صفحة من البوالص مع دعم الفلترة، البحث، الترتيب، والتجزئة.
    /// </summary>
    public async Task<PagedResult<BillOfLadingDto>> GetAllAsync(BillOfLadingQueryParameters query, CancellationToken cancellationToken = default)
    {
        var queryable = _context.BillOfLadings.AsNoTracking();

        if (query.PackageId.HasValue)
        {
            queryable = queryable.Where(b => b.PackageId == query.PackageId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            queryable = queryable.Where(b =>
                b.BillNumber.Contains(term) ||
                (b.IssuedBy != null && b.IssuedBy.Contains(term)));
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = ApplySorting(queryable, query.SortBy, query.SortOrder);

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(b => new BillOfLadingDto
            {
                Id = b.Id,
                BillNumber = b.BillNumber,
                IssuedAt = b.IssuedAt,
                IssuedBy = b.IssuedBy,
                DeliveryFee = b.DeliveryFee,
                Notes = b.Notes,
                PackageId = b.PackageId,
                PackageTrackingNumber = b.Package.TrackingNumber,
                PackageDescription = b.Package.Description,
                ReceiverName = b.Package.ReceiverName,
                CustomerId = b.Package.CustomerId,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<BillOfLadingDto>(items, totalCount, query.Page, query.PageSize);
    }

    /// <summary>
    /// يجلب بوليصة واحدة بمعرّفها.
    /// </summary>
    public async Task<BillOfLadingDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dto = await _context.BillOfLadings
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BillOfLadingDto
            {
                Id = b.Id,
                BillNumber = b.BillNumber,
                IssuedAt = b.IssuedAt,
                IssuedBy = b.IssuedBy,
                DeliveryFee = b.DeliveryFee,
                Notes = b.Notes,
                PackageId = b.PackageId,
                PackageTrackingNumber = b.Package.TrackingNumber,
                PackageDescription = b.Package.Description,
                ReceiverName = b.Package.ReceiverName,
                CustomerId = b.Package.CustomerId,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            throw NotFoundException.ForEntity("BillOfLading", id);
        }

        return dto;
    }

    /// <summary>
    /// يجلب بوليصة البضاعة (علاقة 1:1).
    /// يُرجع 404 "Package not found" لو البضاعة غير موجودة،
    /// و404 "BillOfLading for Package" لو البضاعة موجودة بدون بوليصة.
    /// </summary>
    public async Task<BillOfLadingDto> GetByPackageIdAsync(int packageId, CancellationToken cancellationToken = default)
    {
        var dto = await _context.BillOfLadings
            .AsNoTracking()
            .Where(b => b.PackageId == packageId)
            .Select(b => new BillOfLadingDto
            {
                Id = b.Id,
                BillNumber = b.BillNumber,
                IssuedAt = b.IssuedAt,
                IssuedBy = b.IssuedBy,
                DeliveryFee = b.DeliveryFee,
                Notes = b.Notes,
                PackageId = b.PackageId,
                PackageTrackingNumber = b.Package.TrackingNumber,
                PackageDescription = b.Package.Description,
                ReceiverName = b.Package.ReceiverName,
                CustomerId = b.Package.CustomerId,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is not null)
        {
            return dto;
        }

        // لا بوليصة — نميّز بين "البضاعة غير موجودة" و"البضاعة موجودة بدون بوليصة".
        var packageExists = await _context.Packages
            .AnyAsync(p => p.Id == packageId, cancellationToken);

        if (!packageExists)
        {
            throw NotFoundException.ForEntity("Package", packageId);
        }

        throw NotFoundException.ForEntity("BillOfLading for Package", packageId);
    }

    /// <summary>
    /// ينشئ بوليصة جديدة (مع فحص البضاعة + عدم وجود بوليصة سابقة + تفرّد BillNumber).
    /// </summary>
    public async Task<BillOfLadingDto> CreateAsync(CreateBillOfLadingRequest request, CancellationToken cancellationToken = default)
    {
        // 1) تحميل البضاعة مع بوليصتها (إن وُجدت) — للتحقق من الوجود وعدم التكرار.
        var package = await _context.Packages
            .Include(p => p.BillOfLading)
            .FirstOrDefaultAsync(p => p.Id == request.PackageId, cancellationToken);

        if (package is null)
        {
            throw NotFoundException.ForEntity("Package", request.PackageId);
        }

        if (package.BillOfLading is not null)
        {
            // يطبّق الـUnique index على PackageId بلطف (رسالة عربية بدل 500).
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(CreateBillOfLadingRequest.PackageId),
                    $"البضاعة {request.PackageId} لها بوليصة بالفعل (رقم: {package.BillOfLading.BillNumber}). " +
                    "لا يمكن إصدار بوليصة ثانية لنفس البضاعة.")
            });
        }

        // 2) فحص/توليد BillNumber.
        string billNumber;

        if (!string.IsNullOrWhiteSpace(request.BillNumber))
        {
            billNumber = request.BillNumber.Trim();

            var isDuplicate = await _context.BillOfLadings
                .AnyAsync(b => b.BillNumber == billNumber, cancellationToken);

            if (isDuplicate)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(
                        nameof(CreateBillOfLadingRequest.BillNumber),
                        "رقم البوليصة مستخدم بالفعل. اختر رقماً آخر أو اتركه فارغاً للتوليد التلقائي.")
                });
            }
        }
        else
        {
            billNumber = GenerateBillNumber();
        }

        // 3) إنشاء الكيان.
        var billOfLading = new BillOfLading
        {
            BillNumber = billNumber,
            IssuedAt = request.IssuedAt ?? DateTime.UtcNow,
            IssuedBy = request.IssuedBy,
            DeliveryFee = request.DeliveryFee,
            Notes = request.Notes,
            PackageId = request.PackageId,
            CreatedAt = DateTime.UtcNow
        };

        _context.BillOfLadings.Add(billOfLading);
        await _context.SaveChangesAsync(cancellationToken);

        // 4) إرجاع DTO (باستخدام الـpackage المُحمَّل مسبقاً).
        return MapToDto(billOfLading, package);
    }

    /// <summary>
    /// يحدّث بوليصة موجودة (IssuedBy, DeliveryFee, Notes فقط).
    /// </summary>
    public async Task<BillOfLadingDto> UpdateAsync(int id, UpdateBillOfLadingRequest request, CancellationToken cancellationToken = default)
    {
        var billOfLading = await _context.BillOfLadings
            .Include(b => b.Package)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (billOfLading is null)
        {
            throw NotFoundException.ForEntity("BillOfLading", id);
        }

        billOfLading.IssuedBy = request.IssuedBy;
        billOfLading.DeliveryFee = request.DeliveryFee;
        billOfLading.Notes = request.Notes;
        billOfLading.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(billOfLading, billOfLading.Package);
    }

    /// <summary>
    /// يحذف بوليصة (يسمح بإعادة الإصدار لاحقاً).
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var billOfLading = await _context.BillOfLadings
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (billOfLading is null)
        {
            throw NotFoundException.ForEntity("BillOfLading", id);
        }

        _context.BillOfLadings.Remove(billOfLading);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// يحوّل كيان BillOfLading + كيان Package (المحمّل مسبقاً) إلى BillOfLadingDto.
    /// </summary>
    private static BillOfLadingDto MapToDto(BillOfLading billOfLading, Package package)
    {
        return new BillOfLadingDto
        {
            Id = billOfLading.Id,
            BillNumber = billOfLading.BillNumber,
            IssuedAt = billOfLading.IssuedAt,
            IssuedBy = billOfLading.IssuedBy,
            DeliveryFee = billOfLading.DeliveryFee,
            Notes = billOfLading.Notes,
            PackageId = billOfLading.PackageId,
            PackageTrackingNumber = package.TrackingNumber,
            PackageDescription = package.Description,
            ReceiverName = package.ReceiverName,
            CustomerId = package.CustomerId,
            CreatedAt = billOfLading.CreatedAt,
            UpdatedAt = billOfLading.UpdatedAt
        };
    }

    /// <summary>
    /// يولّد رقم بوليصة فريداً بصيغة BOL-yyyyMMddHHmmss-XXXX.
    /// </summary>
    private static string GenerateBillNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();
        return $"BOL-{timestamp}-{random}";
    }

    /// <summary>
    /// يطبّق الترتيب على IQueryable حسب اسم الحقل واتجاه الترتيب.
    /// </summary>
    private static IQueryable<BillOfLading> ApplySorting(
        IQueryable<BillOfLading> queryable,
        string sortBy,
        string sortOrder)
    {
        var descending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        var normalizedSortBy = (sortBy ?? string.Empty).ToLowerInvariant();

        return normalizedSortBy switch
        {
            "billnumber" => descending
                ? queryable.OrderByDescending(b => b.BillNumber)
                : queryable.OrderBy(b => b.BillNumber),

            "issuedat" => descending
                ? queryable.OrderByDescending(b => b.IssuedAt)
                : queryable.OrderBy(b => b.IssuedAt),

            "deliveryfee" => descending
                ? queryable.OrderByDescending(b => b.DeliveryFee)
                : queryable.OrderBy(b => b.DeliveryFee),

            "updatedat" => descending
                ? queryable.OrderByDescending(b => b.UpdatedAt)
                : queryable.OrderBy(b => b.UpdatedAt),

            // الافتراضي: createdAt desc.
            _ => descending
                ? queryable.OrderByDescending(b => b.CreatedAt)
                : queryable.OrderBy(b => b.CreatedAt)
        };
    }
}