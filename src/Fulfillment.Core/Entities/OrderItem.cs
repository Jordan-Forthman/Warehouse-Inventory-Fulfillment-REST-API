using Fulfillment.Core.Exceptions;


using System;
using System.Reflection;

namespace Fulfillment.Core.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    // Navigation properties for EF Core
    public Order? Order { get; private set; }
    public Product? Product { get; private set; }

    private OrderItem() { }

    public OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty)
            throw new DomainValidationException("OrderId must be specified.");
        if (productId == Guid.Empty)
            throw new DomainValidationException("ProductId must be specified.");
        if (quantity <= 0)
            throw new DomainValidationException("Quantity must be greater than zero.");
        if (unitPrice <= 0)
            throw new DomainValidationException("Unit price must be greater than zero.");

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}