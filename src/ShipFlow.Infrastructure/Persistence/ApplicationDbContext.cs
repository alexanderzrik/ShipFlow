using Microsoft.EntityFrameworkCore;
using ShipFlow.Application.Common.Interfaces;
using ShipFlow.Domain.Entities;
using System.Reflection;

namespace ShipFlow.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<BillOfLading> BillOfLadings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CurrencyRate> CurrencyRates { get; set; } // الجديد

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    Task<int> IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}