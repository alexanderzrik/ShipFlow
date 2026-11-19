using Microsoft.EntityFrameworkCore;
using ShipFlow.Domain.Entities;

namespace ShipFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Package> Packages { get; }
    DbSet<Shipment> Shipments { get; }
    DbSet<BillOfLading> BillOfLadings { get; }
    DbSet<User> Users { get; }
    DbSet<CurrencyRate> CurrencyRates { get; } // الجديد

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}