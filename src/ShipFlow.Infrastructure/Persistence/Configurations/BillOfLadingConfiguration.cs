using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipFlow.Domain.Entities;

namespace ShipFlow.Infrastructure.Persistence.Configurations;

/// <summary>
/// إعدادات جدول بوالص الشحن (BillOfLading).
/// </summary>
public class BillOfLadingConfiguration : IEntityTypeConfiguration<BillOfLading>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BillOfLading> builder)
    {
        builder.ToTable("BillOfLadings");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BillNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(b => b.BillNumber)
            .IsUnique()
            .HasDatabaseName("IX_BillOfLadings_BillNumber");

        builder.Property(b => b.IssuedAt)
            .IsRequired();

        builder.Property(b => b.IssuedBy)
            .HasMaxLength(200);

        builder.Property(b => b.DeliveryFee)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.PackageId)
            .IsRequired();

        builder.HasIndex(b => b.PackageId)
            .IsUnique()
            .HasDatabaseName("IX_BillOfLadings_PackageId");

        builder.HasOne(b => b.Package)
            .WithOne(p => p.BillOfLading)
            .HasForeignKey<BillOfLading>(b => b.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}