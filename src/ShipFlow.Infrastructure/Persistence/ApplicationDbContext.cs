using Microsoft.EntityFrameworkCore;
using ShipFlow.Application.Common.Interfaces;
using ShipFlow.Domain.Entities;
using System.Reflection;

namespace ShipFlow.Infrastructure.Persistence;

/// <summary>
/// سياق قاعدة البيانات الرئيسي للتطبيق.
/// يمثّل جلسة الاتصال مع SQL Server، ويوفّر الوصول لكل الجداول (DbSets).
/// يطبّق واجهة IApplicationDbContext لكي تستخدمه طبقة Application بدون معرفة EF Core.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// ينشئ نسخة جديدة من السياق.
    /// </summary>
    /// <param name="options">إعدادات السياق (Connection String، المزوّد).</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>جدول الزبائن.</summary>
    public DbSet<Customer> Customers { get; set; }

    /// <summary>جدول البضائع/الطرود.</summary>
    public DbSet<Package> Packages { get; set; }

    /// <summary>جدول الشحنات (الرحلات).</summary>
    public DbSet<Shipment> Shipments { get; set; }

    /// <summary>جدول بوالص الشحن.</summary>
    public DbSet<BillOfLading> BillOfLadings { get; set; }

    /// <summary>جدول المستخدمين (مشغّلي النظام).</summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// يطبّق كل إعدادات الكيانات (IEntityTypeConfiguration) الموجودة بهذا الـAssembly.
    /// </summary>
    /// <param name="modelBuilder">الباني الخاص بالموديل.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// يحفظ كل التغييرات المعلّقة على قاعدة البيانات.
    /// </summary>
    /// <param name="cancellationToken">رمز إلغاء العملية.</param>
    /// <returns>عدد السجلات المتأثرة.</returns>
    Task<int> IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}