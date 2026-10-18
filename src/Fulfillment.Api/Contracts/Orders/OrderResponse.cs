using System;
using System.Collections.Generic;

namespace Fulfillment.Api.Contracts.Orders;

public record OrderItemResponse(Guid Id, Guid ProductId, int Quantity, decimal UnitPrice);

public record OrderResponse(Guid Id, string CustomerOrderNumber, string Status, DateTime CreatedAtUtc, List<OrderItemResponse> Items);