using System;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Data;
using BAMF_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BAMF_API.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _ctx;
    public InventoryRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Inventory inv, CancellationToken ct = default)
    {
        await _ctx.Inventories.AddAsync(inv, ct);
    }

    public async Task<Inventory?> GetByVariantIdAsync(Guid variantId, CancellationToken ct = default)
    {
        return await _ctx.Inventories.FirstOrDefaultAsync(i => i.VariantId == variantId, ct);
    }

    public void Update(Inventory inv)
    {
        _ctx.Inventories.Update(inv);
    }
}