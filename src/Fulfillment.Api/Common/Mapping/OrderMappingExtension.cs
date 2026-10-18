using Fulfillment.Api.Contracts.Orders;
using Fulfillment.Core.Entities;

namespace Fulfillment.Api.Common.Mapping;

public static class OrderMappingExtensions
{
    public static OrderResponse ToResponse(this Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerOrderNumber,
            order.Status.ToString(),
            order.CreatedAtUtc,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductId,
                i.Quantity,
                i.UnitPrice
             )).ToList()
        );
    }
}