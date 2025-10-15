using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.DTOs.Requests.Orders;
using BAMF_API.Models;
using BAMF_API.Repositories;
using BAMF_API.CrossCutting;

namespace BAMF_API.Services;

public class OrdersService : IOrdersService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IVariantRepository _variantRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IInventoryTransactionRepository _txRepo;
    private readonly IUnitOfWork _uow;

    public OrdersService(
        IOrderRepository orderRepo,
        IVariantRepository variantRepo,
        IInventoryRepository inventoryRepo,
        IInventoryTransactionRepository txRepo,
        IUnitOfWork uow)
    {
        _orderRepo = orderRepo;
        _variantRepo = variantRepo;
        _inventoryRepo = inventoryRepo;
        _txRepo = txRepo;
        _uow = uow;
    }

    public async Task<int> CreateOrderAsync(OrderCreateDto dto, CancellationToken ct = default)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (dto.Lines == null || !dto.Lines.Any()) throw new ArgumentException("Order must contain at least one line.");

        var orderNo = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("n").Substring(0,6)}";
        var resolvedLines = new List<OrderLine>();

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            decimal total = 0m;

            foreach (var l in dto.Lines)
            {
                var variant = await _variantRepo.GetBySkuAsync(l.Sku, ct);
                if (variant == null) throw new InvalidOperationException($"SKU not found: {l.Sku}");
                if (variant.IsDeleted) throw new InvalidOperationException($"SKU not available: {l.Sku}");
                var unitPrice = variant.Price;
                var inventory = await _inventoryRepo.GetByVariantIdAsync(variant.Id, ct);
                var existingQty = inventory?.Quantity ?? 0;
                if (existingQty < l.Quantity) throw new InvalidOperationException($"Insufficient stock for SKU {l.Sku}. Requested {l.Quantity}, available {existingQty}");
                inventory!.Quantity = existingQty - l.Quantity;
                _inventoryRepo.Update(inventory);

                var tx = new InventoryTransaction
                {
                    InventoryId = inventory.Id,
                    Change = -l.Quantity,
                    Type = InventoryTransactionType.Sale,
                    ReferenceId = orderNo,
                    CreatedAt = DateTime.UtcNow
                };
                await _txRepo.AddAsync(tx, ct);

                var ol = new OrderLine
                {
                    VariantId = variant.Id,
                    Sku = l.Sku,
                    Quantity = l.Quantity,
                    UnitPrice = unitPrice,
                    LineTotal = unitPrice * l.Quantity
                };
                resolvedLines.Add(ol);
                total += ol.LineTotal;
            }

            var order = new Order
            {
                OrderNo = orderNo,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Created,
                Total = total,
                Lines = resolvedLines,
                ShippingAddress = dto.ShippingAddress,
                BillingAddress = dto.BillingAddress
            };

            await _orderRepo.AddAsync(order, ct);
        }, ct);

        var created = await _orderRepo.GetByOrderNoAsync(orderNo, ct);
        if (created == null) throw new Exception("Order creation failed (could not retrieve created order).");

        return created.Id;
    }

    public Task UpdateOrderAsync(int id, object dto, CancellationToken ct = default) => throw new NotImplementedException();
    public Task DeleteOrderAsync(int id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default) => _orderRepo.GetByIdAsync(id);
    public Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken ct = default) => _orderRepo.GetByOrderNoAsync(orderNo);
    public Task<List<Order>> GetByEmailAsync(string email, CancellationToken ct = default) => _orderRepo.GetByEmailAsync(email);
    public Task<List<Order>> ListAllAsync(CancellationToken ct = default) => _orderRepo.ListAllAsync(ct);
}