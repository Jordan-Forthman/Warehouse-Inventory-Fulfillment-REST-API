using Fulfillment.Core.Exceptions;
using System;
using System.Reflection;

namespace Fulfillment.Core.Entities;

public class InventoryItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid WarehouseLocationId { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int QuantityAllocated { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public Product? Product { get; private set; }
    public WarehouseLocation? WarehouseLocation { get; private set; }

    private InventoryItem() { }

    public InventoryItem(Guid productId, Guid warehouseLocationId)
    {
        if (productId == Guid.Empty)
            throw new DomainValidationException("Product ID cannot be empty.");

        if (warehouseLocationId == Guid.Empty)
            throw new DomainValidationException("Warehouse Location ID cannot be empty.");

        Id = Guid.NewGuid();
        ProductId = productId;
        WarehouseLocationId = warehouseLocationId;
        QuantityOnHand = 0;
        QuantityAllocated = 0;
    }

    public int QuantityAvailable => QuantityOnHand - QuantityAllocated;

    public void ReceiveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Received quantity must be greater than zero.");
        QuantityOnHand += quantity;
    }

    public void AllocateStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Allocation quantity must be greater than 0.");

        if (quantity > QuantityAvailable)
            throw new DomainValidationException("Not enough available stock to allocate.");
        QuantityAllocated += quantity;
    }

    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Release quantity must be greater than zero.");

        if (quantity > QuantityAllocated)
            throw new DomainValidationException("Cannot release more stock than is allocated.");

        QuantityAllocated -= quantity;
    }

    public void DeductShippedStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Shipped quantity must be greater than zero.");

        if (quantity > QuantityAllocated)
            throw new DomainValidationException("Cannot ship more stock than is allocated.");

        QuantityOnHand -= quantity;
        QuantityAllocated -= quantity;
    }
}