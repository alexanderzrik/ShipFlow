using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipFlow.Domain.Entities;
using ShipFlow.Domain.Enums;

namespace ShipFlow.Infrastructure.Persistence.Configurations;

/// <summary>
/// إعدادات جدول البضائع (Package) — الأطوال، الفهارس، العلاقات.
/// </summary>
public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("Packages");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.TrackingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.TrackingNumber)
            .IsUnique()
            .HasDatabaseName("IX_Packages_TrackingNumber");

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Weight)
            .IsRequired()
            .HasColumnType("decimal(18,3)");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(PackageStatus.Received);

        builder.Property(p => p.ReceiverName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.ReceiverPhone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.DestinationCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CustomerId)
            .IsRequired();

        builder.Property(p => p.ShipmentId);

        builder.HasIndex(p => p.ShipmentId)
            .HasDatabaseName("IX_Packages_ShipmentId");

        builder.HasOne(p => p.Shipment)
            .WithMany(s => s.Packages)
            .HasForeignKey(p => p.ShipmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.BillOfLading)
            .WithOne(b => b.Package)
            .HasForeignKey<BillOfLading>(b => b.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}