using Microsoft.EntityFrameworkCore;
using ShipFlow.Application.Common.Exceptions;
using ShipFlow.Application.Common.Interfaces;
using ShipFlow.Application.Common.Models;
using ShipFlow.Application.Features.Customers.Dtos;
using ShipFlow.Application.Features.Customers.Queries;
using ShipFlow.Domain.Entities;

namespace ShipFlow.Application.Features.Customers.Services;

/// <summary>
/// تنفيذ خدمة إدارة الزبائن.
/// يعتمد على IApplicationDbContext للحصول على الوصول لقاعدة البيانات
/// دون معرفة تفاصيل التنفيذ (EF Core / SQL Server).
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// ينشئ نسخة جديدة من خدمة الزبائن.
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات (يُحقن عبر DI).</param>
    public CustomerService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// يجلب زبوناً واحداً بواسطة المعرف.
    /// </summary>
    /// <param name="id">معرف الزبون.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>بيانات الزبون.</returns>
    /// <exception cref="NotFoundException">إذا لم يوجد الزبون.</exception>
    public async Task<CustomerDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (customer is null)
        {
            throw NotFoundException.ForEntity("Customer", id);
        }

        return MapToDto(customer);
    }

    /// <summary>
    /// يجلب صفحة من الزبائن مع دعم الفلترة (IsActive)، البحث (FullName/PhoneNumber)،
    /// والترتيب الديناميكي (sortBy/sortOrder)، مع تجزئة (Skip/Take) وإرجاع PagedResult.
    /// </summary>
    /// <param name="query">معاملات الاستعلام.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>نتيجة مُجزَّأة تحتوي العناصر + معلومات الصفحة.</returns>
    public async Task<PagedResult<CustomerDto>> GetAllAsync(CustomerQueryParameters query, CancellationToken cancellationToken = default)
    {
        // 1) نقطة البداية: قراءة فقط (بدون تتبّع) — أداء أفضل في القوائم.
        var queryable = _context.Customers.AsNoTracking();

        // 2) فلترة حسب حالة التنشيط (اختياري — null = الكل).
        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(c => c.IsActive == query.IsActive.Value);
        }

        // 3) فلترة البحث النصي (اختياري — null/فراغ = بدون بحث).
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchTerm = query.Search.Trim();
            queryable = queryable.Where(c =>
                c.FullName.Contains(searchTerm) ||
                c.PhoneNumber.Contains(searchTerm));
        }

        // 4) عدد العناصر الكلي قبل تطبيق Skip/Take (لحساب TotalPages).
        var totalCount = await queryable.CountAsync(cancellationToken);

        // 5) ترتيب ديناميكي حسب sortBy + sortOrder.
        queryable = ApplySorting(queryable, query.SortBy, query.SortOrder);

        // 6) تطبيق Pagination ثم الإسقاط إلى CustomerDto.
        //    نُسقِط بعد Skip/Take لتقليل الحقول المسحوبة من DB.
        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName,
                PhoneNumber = c.PhoneNumber,
                AlternativePhoneNumber = c.AlternativePhoneNumber,
                Address = c.Address,
                Notes = c.Notes,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        // 7) إرجاع النتيجة المُجزَّأة (TotalPages/HasNext تُحسب تلقائياً).
        return new PagedResult<CustomerDto>(items, totalCount, query.Page, query.PageSize);
    }

    /// <summary>
    /// ينشئ زبوناً جديداً.
    /// </summary>
    /// <param name="request">بيانات الزبون الجديد.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>الزبون المنشأ.</returns>
    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            AlternativePhoneNumber = request.AlternativePhoneNumber,
            Address = request.Address,
            Notes = request.Notes,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(customer);
    }

    /// <summary>
    /// يحدّث بيانات زبون موجود.
    /// </summary>
    /// <param name="id">معرف الزبون.</param>
    /// <param name="request">البيانات الجديدة.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>الزبون بعد التحديث.</returns>
    /// <exception cref="NotFoundException">إذا لم يوجد الزبون.</exception>
    public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (customer is null)
        {
            throw NotFoundException.ForEntity("Customer", id);
        }

        customer.FullName = request.FullName;
        customer.PhoneNumber = request.PhoneNumber;
        customer.AlternativePhoneNumber = request.AlternativePhoneNumber;
        customer.Address = request.Address;
        customer.Notes = request.Notes;
        customer.IsActive = request.IsActive;
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(customer);
    }

    /// <summary>
    /// يحذف زبوناً موجود.
    /// </summary>
    /// <param name="id">معرف الزبون.</param>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <exception cref="NotFoundException">إذا لم يوجد الزبون.</exception>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (customer is null)
        {
            throw NotFoundException.ForEntity("Customer", id);
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// يحوّل كيان Customer إلى CustomerDto (فصل الكيان عن طبقة العرض).
    /// </summary>
    /// <param name="customer">كيان الزبون.</param>
    /// <returns>DTO الزبون.</returns>
    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            PhoneNumber = customer.PhoneNumber,
            AlternativePhoneNumber = customer.AlternativePhoneNumber,
            Address = customer.Address,
            Notes = customer.Notes,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    /// <summary>
    /// يطبّق الترتيب على IQueryable حسب اسم الحقل واتجاه الترتيب.
    /// الـValidator في طبقة الـApplication يضمن مسبقاً أن sortBy/sortOrder من White-list،
    /// لكن نضيف هنا احتياطاً (fallback إلى createdAt) للاستدعاءات المباشرة للخدمة.
    /// </summary>
    /// <param name="queryable">الاستعلام الأصلي.</param>
    /// <param name="sortBy">اسم الحقل (fullName|phoneNumber|createdAt|updatedAt).</param>
    /// <param name="sortOrder">الاتجاه (asc|desc).</param>
    /// <returns>نفس الاستعلام بعد الترتيب.</returns>
    private static IQueryable<Customer> ApplySorting(
        IQueryable<Customer> queryable,
        string sortBy,
        string sortOrder)
    {
        var descending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        var normalizedSortBy = (sortBy ?? string.Empty).ToLowerInvariant();

        return normalizedSortBy switch
        {
            "fullname" => descending
                ? queryable.OrderByDescending(c => c.FullName)
                : queryable.OrderBy(c => c.FullName),

            "phonenumber" => descending
                ? queryable.OrderByDescending(c => c.PhoneNumber)
                : queryable.OrderBy(c => c.PhoneNumber),

            "updatedat" => descending
                ? queryable.OrderByDescending(c => c.UpdatedAt)
                : queryable.OrderBy(c => c.UpdatedAt),

            // الافتراضي: createdAt desc (الأحدث أولاً) — يطابق قرار المستخدم.
            _ => descending
                ? queryable.OrderByDescending(c => c.CreatedAt)
                : queryable.OrderBy(c => c.CreatedAt)
        };
    }
}