using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Fulfillment.Api.Contracts.Orders;
using Fulfillment.Core.Entities;
using Fulfillment.Infrastructure.Data;
using Fulfillment.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fulfillment.Tests.Integration;

public class AllocationConcurrencyTests : IClassFixture<FulfillmentTestFactory>, IAsyncLifetime
{
    private readonly FulfillmentTestFactory _factory;
    private readonly HttpClient _client;
    private Guid _testProductId;

    public AllocationConcurrencyTests(FulfillmentTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.InitializeDatabaseAsync();

        // Seed baseline test state: 1 Product, 1 Location, 1 InventoryItem with 10 units
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FulfillmentDbContext>();

        var location = new WarehouseLocation("TEST-A", "01", "B01", 100);
        var product = new Product("SKU-CONCUR-01", "Concurrency Bearing", 50.00m);
        context.WarehouseLocations.Add(location);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var inventory = new InventoryItem(product.Id, location.Id);
        inventory.ReceiveStock(10);
        context.InventoryItems.Add(inventory);
        await context.SaveChangesAsync();

        _testProductId = product.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ConcurrentAllocations_WhenStockIsLimited_OnlyOneSucceeds()
    {
        // 1. Arrange: Create two separate pending orders for 10 units each
        var order1Response = await _client.PostAsJsonAsync("/api/v1/orders", new CreateOrderRequest(
            "ORD-CONCUR-A",
            new List<CreateOrderItemRequest> { new(_testProductId, 10) }
        ));
        order1Response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order1 = await order1Response.Content.ReadFromJsonAsync<OrderResponse>();

        var order2Response = await _client.PostAsJsonAsync("/api/v1/orders", new CreateOrderRequest(
            "ORD-CONCUR-B",
            new List<CreateOrderItemRequest> { new(_testProductId, 10) }
        ));
        order2Response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order2 = await order2Response.Content.ReadFromJsonAsync<OrderResponse>();

        // 2. Act: Fire allocation requests concurrently
        var allocationTask1 = _client.PostAsync($"/api/v1/orders/{order1!.Id}/allocate", null);
        var allocationTask2 = _client.PostAsync($"/api/v1/orders/{order2!.Id}/allocate", null);

        var responses = await Task.WhenAll(allocationTask1, allocationTask2);

        // 3. Assert HTTP Statuses: One success, one failure
        var successfulAllocations = responses.Where(r => r.StatusCode == HttpStatusCode.OK).ToList();
        var failedAllocations = responses.Where(r => r.StatusCode is HttpStatusCode.Conflict or HttpStatusCode.BadRequest).ToList();

        successfulAllocations.Should().ContainSingle("only one order should win the race condition");
        failedAllocations.Should().ContainSingle("the losing request must be rejected");

        // 4. Assert Database State: Total allocated stock must not exceed total on-hand (10)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FulfillmentDbContext>();

        var inventoryItem = context.InventoryItems.First(i => i.ProductId == _testProductId);
        inventoryItem.QuantityOnHand.Should().Be(10);
        inventoryItem.QuantityAllocated.Should().Be(10);
        inventoryItem.QuantityAvailable.Should().Be(0);
    }
}