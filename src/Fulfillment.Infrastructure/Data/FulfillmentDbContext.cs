using Fulfillment.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace Fulfillment.Infrastructure.Data;

public class FulfillmentDbContext : DbContext
{
    public FulfillmentDbContext(DbContextOptions<FulfillmentDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FulfillmentDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}