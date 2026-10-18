using Fulfillment.Core.Exceptions;
using System;
using System.Web;

namespace Fulfillment.Core.Entities;

public class WarehouseLocation
{
	public Guid Id { get; private set; }
	public string Aisle { get; private set; } = string.Empty;
	public string Shelf { get; private set; } = string.Empty;
	public string Bin { get; private set; } = string.Empty;
	public int MaxCapacity { get; private set; }

	private WarehouseLocation() { }

	public WarehouseLocation(string aisle, string shelf, string bin, int maxCapacity)
	{
		Id = Guid.NewGuid();
		SetLocationDetails(aisle, shelf, bin);
		SetMaxCapacity(maxCapacity);
	}

	public void SetLocationDetails(string aisle, string shelf, string bin)
	{
        if (string.IsNullOrWhiteSpace(aisle) || string.IsNullOrWhiteSpace(shelf) || string.IsNullOrWhiteSpace(bin))
			throw new DomainValidationException("Aisle, Shelf, and Bin coordinates cannot be empty.");

		Aisle = aisle.Trim().ToUpperInvariant();
		Shelf = shelf.Trim().ToUpperInvariant();
		Bin = bin.Trim().ToUpperInvariant();
	}

	public void SetMaxCapacity(int maxCapacity)
	{
		if (maxCapacity <= 0)
			throw new DomainValidationException("Max capacity must be greater than zero.");
		MaxCapacity = maxCapacity;
	}
}