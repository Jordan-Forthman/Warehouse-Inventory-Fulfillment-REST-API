using Fulfillment.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fulfillment.Tests.Infrastructure;

public class FulfillmentTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Locate existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<FulfillmentDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register test database configuration
            services.AddDbContext<FulfillmentDbContext>(options =>
            {
                // Connect to local SQL Server test database
                options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=WarehouseFulfillmentTestDb;Trusted_Connection=True;TrustServerCertificate=True;");
            });
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FulfillmentDbContext>();

        // Ensure test database exists and applies latest schema
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }
}