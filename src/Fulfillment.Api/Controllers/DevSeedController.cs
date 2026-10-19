using Fulfillment.Core.Entities;
using Fulfillment.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Fulfillment.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DevSeedController : ControllerBase
{
    private readonly FulfillmentDbContext _context;

    public DevSeedController(FulfillmentDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> SeedBaselineData(CancellationToken ct)
    {
        // 1. Check if inventory item already exists in the database
        var existingInventory = await _context.InventoryItems.FirstOrDefaultAsync(ct);
        if (existingInventory is not null)
        {
            return Ok(new
            {
                Message = "Database already contains seeded inventory.",
                ProductId = existingInventory.ProductId,
                WarehouseLocationId = existingInventory.WarehouseLocationId,
                InitialOnHand = existingInventory.QuantityOnHand
            });
        }

        var location = await _context.WarehouseLocations.FirstOrDefaultAsync(ct);
        if (location is null)
        {
            location = new WarehouseLocation("A", "01", "B01", 500);
            _context.WarehouseLocations.Add(location);
        }

        var product = await _context.Products.FirstOrDefaultAsync(ct);
        if (product is null)
        {
            product = new Product("SKU-CORE-10", "Electric Motor", 189.50m);
            _context.Products.Add(product);
        }

        await _context.SaveChangesAsync(ct);

        var inventory = new InventoryItem(product.Id, location.Id);
        inventory.ReceiveStock(50); // Seed with 50 units on hand

        _context.InventoryItems.Add(inventory);
        await _context.SaveChangesAsync(ct);

        return Ok(new
        {
            Message = "Seed completed successfully.",
            ProductId = product.Id,
            WarehouseLocationId = location.Id,
            InitialOnHand = inventory.QuantityOnHand
        });
    }
}