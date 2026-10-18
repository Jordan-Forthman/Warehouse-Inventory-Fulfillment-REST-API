using Fulfillment.Core.Exceptions;
using Fulfillment.Core.Enums;
using System.Collections.Generic;
using System;
using System.Security.Policy;
using System.Linq;

namespace Fulfillment.Core.Entities;

public class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public String CustomerOrderNumber { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public Order(string customerOrderNumber)
    {
        if (string.IsNullOrWhiteSpace(customerOrderNumber))
            throw new DomainValidationException("Customer order number must be specified.");

        Id = Guid.NewGuid();
        CustomerOrderNumber = customerOrderNumber.Trim().ToUpperInvariant();
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddOrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainValidationException("Cannot add items to an order that is not in Pending status.");

        if (_items.Any(i => i.ProductId == productId))
            throw new DomainValidationException("Product already exists in this order. Update quantity instead.");

        _items.Add(new OrderItem(Id, productId, quantity, unitPrice));
    }

    public void MarkAllocated()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainValidationException($"Cannot allocate order in '{Status}' status. Must be 'Pending'.");

        if (_items.Count == 0)
            throw new DomainValidationException("Cannot allocate an order with no line items.");

        Status = OrderStatus.Allocated;
    }
    public void MarkShipped()
    {
        if (Status != OrderStatus.Allocated)
            throw new DomainValidationException($"Cannot ship order in '{Status}' status. Must be 'Allocated'.");

        Status = OrderStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new DomainValidationException("Cannot cancel an order that has been shipped.");

        Status = OrderStatus.Cancelled;
    }
}