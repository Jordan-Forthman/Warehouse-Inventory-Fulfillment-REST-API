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
        // Avoid duplicate seeding
        if (await _context.WarehouseLocations.AnyAsync(ct))
            return Ok("Database already contains seeded data.");

        var location = new WarehouseLocation("A", "01", "B01", 500);
        var product = new Product("SKU-CORE-10", "Electric Motor", 189.50m);

        _context.WarehouseLocations.Add(location);
        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);

        var inventory = new InventoryItem(product.Id, location.Id);
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