
using System;

namespace BAMF_API.Models;

public enum InventoryTransactionType
{
    Restock = 1,
    Sale = 2,
    Adjustment = 3
}

public class InventoryTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    public InventoryTransactionType TransactionType { get; set; }
    public int QuantityChange { get; set; }
    public int? ResultingQuantity { get; set; }
    public string? ReferenceId { get; set; } // e.g., OrderId

    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
