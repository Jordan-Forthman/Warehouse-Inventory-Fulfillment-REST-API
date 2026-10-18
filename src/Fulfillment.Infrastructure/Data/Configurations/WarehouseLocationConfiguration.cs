using Fulfillment.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fulfillment.Infrastructure.Data.Configurations;

public class WarehouseLocationConfiguration : IEntityTypeConfiguration<WarehouseLocation>
{
	public void Configure(EntityTypeBuilder<WarehouseLocation> builder)
	{
		builder.ToTable("WarehouseLocations");
		builder.HasKey(w => w.Id);
		builder.Property(w => w.Aisle)
			.IsRequired()
			.HasMaxLength(10);
		builder.Property(w => w.Shelf)
			.IsRequired()
			.HasMaxLength(10);
		builder.Property(w => w.Bin)
			.IsRequired()
			.HasMaxLength(10);
		builder.Property(w => w.MaxCapacity)
			.IsRequired();
		builder.HasIndex(w => new { w.Aisle, w.Shelf, w.Bin })
			.IsUnique();
	}
}