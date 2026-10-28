using Microsoft.EntityFrameworkCore;
using ShipFlow.Domain.Entities;

namespace ShipFlow.Application.Common.Interfaces;

/// <summary>
/// واجهة سياق قاعدة البيانات.
/// تعرّف ما يحتاجه Application من قاعدة البيانات (بدون معرفة التنفيذ الفعلي).
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>جدول الزبائن.</summary>
    DbSet<Customer> Customers { get; }

    /// <summary>جدول البضائع.</summary>
    DbSet<Package> Packages { get; }

    /// <summary>جدول الشحنات.</summary>
    DbSet<Shipment> Shipments { get; }

    /// <summary>جدول بوالص الشحن.</summary>
    DbSet<BillOfLading> BillOfLadings { get; }

    /// <summary>جدول المستخدمين (مشغّلي النظام).</summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// يحفظ كل التغييرات المعلّقة على قاعدة البيانات.
    /// </summary>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>عدد السجلات المتأثرة.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}