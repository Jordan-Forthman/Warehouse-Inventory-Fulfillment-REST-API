using System;
using System.Collections.Generic;

namespace Fulfillment.Api.Contracts.Orders;

public record CreateOrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderRequest(string CustomerOrderNumber, List<CreateOrderItemRequest> Items);
