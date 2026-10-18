using Fulfillment.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fulfillment.Infrastructure.Data.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
	public void Configure(EntityTypeBuilder<InventoryItem> builder)
	{
		builder.ToTable("InventoryItems");

		builder.HasKey(i => i.Id);
		
		builder.Property(i => i.QuantityOnHand)
			.IsRequired();
		
		builder.Property(i => i.QuantityAllocated)
			.IsRequired();

		builder.Property(i => i.RowVersion)
			.IsRowVersion();
		
		builder.Ignore(i => i.QuantityAvailable);
		
		builder.HasIndex(i => new { i.ProductId, i.WarehouseLocationId })
			.IsUnique();
		
		builder.HasOne(i => i.Product)
			.WithMany()
			.HasForeignKey(i => i.ProductId)
			.OnDelete(DeleteBehavior.Restrict);
		
		builder.HasOne(i => i.WarehouseLocation)
			.WithMany()
			.HasForeignKey(i => i.WarehouseLocationId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}