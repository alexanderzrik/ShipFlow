using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipFlow.Domain.Entities;
using ShipFlow.Domain.Enums;

namespace ShipFlow.Infrastructure.Persistence.Configurations;

/// <summary>
/// إعدادات جدول الشحنات (Shipment).
/// </summary>
public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ShipmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.ShipmentNumber)
            .IsUnique()
            .HasDatabaseName("IX_Shipments_ShipmentNumber");

        builder.Property(s => s.FromCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.ToCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.DriverName)
            .HasMaxLength(200);

        builder.Property(s => s.DriverPhone)
            .HasMaxLength(30);

        builder.Property(s => s.VehicleNumber)
            .HasMaxLength(50);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ShipmentStatus.Loading);

        builder.Property(s => s.DepartureDate);
        builder.Property(s => s.ArrivalDate);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.CreatedAt)
            .IsRequired();
    }
}