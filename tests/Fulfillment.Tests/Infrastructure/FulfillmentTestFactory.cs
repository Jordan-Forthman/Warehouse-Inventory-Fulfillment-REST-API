using Fulfillment.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;

namespace Fulfillment.Tests.Infrastructure;

public class FulfillmentTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<FulfillmentDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<FulfillmentDbContext>(options =>
            {
                options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=FulfillmentTestDb;Trusted_Connection=True;TrustServerCertificate=True");
            });
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = ChannelServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FulfillmentDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }
}