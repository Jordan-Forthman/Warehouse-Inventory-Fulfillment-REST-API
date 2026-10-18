using Fulfillment.Core.Exceptions;
using System;

namespace Fulfillment.Core.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    // Parameterless constructor for materialization
    // Private prevents business code from creating unvalidated empty entities
    private Product() { }

    public Product(string sku, string name, decimal price)
    {
        Id = Guid.NewGuid();
        CreatedAtUtc = DateTime.UtcNow;
        IsActive = true;

        UpdateDetails(sku, name);
        UpdatePrice(price);
    }

    public void UpdateDetails(string sku, string name)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainValidationException("SKU cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Product name cannot be empty.");

        Sku = sku.Trim().ToUpperInvariant();
        Name = name.Trim();
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new DomainValidationException("Price cannot be negative.");

        Price = newPrice;
    }
}