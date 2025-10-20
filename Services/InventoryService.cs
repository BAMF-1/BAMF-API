// BAMF_API/Services/InventoryService.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Interfaces;
using BAMF_API.Interfaces.InventoryInterfaces;
using BAMF_API.Interfaces.ProductInterfaces;
using BAMF_API.Models;

namespace BAMF_API.Services
{
	public class InventoryService : IInventoryService
	{
        private readonly IVariantRepository _variants;
		private readonly IInventoryRepository _inventories;
		private readonly IInventoryTransactionRepository _transactions;
		private readonly IUnitOfWork _uow;

		public InventoryService(
			IVariantRepository variants,
			IInventoryRepository inventories,
			IInventoryTransactionRepository transactions,
			IUnitOfWork uow)
		{
			_variants = variants;
			_inventories = inventories;
			_transactions = transactions;
			_uow = uow;
		}

		public async Task<int> GetGroupQuantityAsync(Guid productGroupId, CancellationToken ct = default)
		{
			var variants = await _variants.ListByGroupAsync(productGroupId, ct);
			int total = 0;
			foreach (var v in variants)
			{
				var inv = await _inventories.GetByVariantIdAsync(v.Id, ct);
				if (inv != null) total += inv.Quantity;
			}
			return total;
		}

		public async Task<int> GetSkuQuantityAsync(string sku, CancellationToken ct = default)
		{
			var variant = await _variants.GetBySkuAsync(sku, ct);
			if (variant == null) return 0;
			var inv = await _inventories.GetByVariantIdAsync(variant.Id, ct);
			return inv?.Quantity ?? 0;
		}

		public async Task AdjustInventoryAsync(
			string sku,
			int delta,
			InventoryTransactionType type,
			string? referenceId = null,
			CancellationToken ct = default)
		{
			if (delta == 0) return;

			// Resolve the variant by SKU
			var variant = await _variants.GetBySkuAsync(sku, ct)
						  ?? throw new InvalidOperationException($"Variant with SKU {sku} not found.");

			// Ensure an inventory record exists
			var inventory = await _inventories.GetByVariantIdAsync(variant.Id, ct);
            if (inventory == null)
            {
                inventory = new Inventory
                {
                    VariantId = variant.Id,
                    Quantity = 0,
                    LowStockThreshold = 0
                };
                await _inventories.AddAsync(inventory, ct);
            }

            // Calculate the new quantity and prevent negative stock
            var newQuantity = inventory.Quantity + delta;
			if (newQuantity < 0)
				throw new InvalidOperationException($"Cannot reduce stock below zero for SKU {sku}.");

			// Apply the change
			inventory.Quantity = newQuantity;
			if (type == InventoryTransactionType.Restock && delta > 0)
				inventory.LastRestockDate = DateTimeOffset.UtcNow;

			// Log the transaction
			var tx = new InventoryTransaction
			{
				InventoryId = inventory.Id,
				TransactionType = type,
				QuantityChange = delta,
				ResultingQuantity = inventory.Quantity,
				ReferenceId = referenceId,
				Timestamp = DateTimeOffset.UtcNow
			};

			// Persist changes
			_inventories.Update(inventory);
			await _transactions.AddAsync(tx, ct);
			await _uow.SaveChangesAsync(ct);
		}
	}
}
