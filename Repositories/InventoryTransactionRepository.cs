using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Data;
using BAMF_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BAMF_API.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly AppDbContext _ctx;
    public InventoryTransactionRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(InventoryTransaction tx, CancellationToken ct = default)
    {
        await _ctx.InventoryTransactions.AddAsync(tx, ct);
    }

    public async Task<IReadOnlyList<InventoryTransaction>> ListByInventoryAsync(Guid inventoryId, int top = 100, CancellationToken ct = default)
    {
        return await _ctx.InventoryTransactions.Where(t => t.InventoryId == inventoryId).OrderByDescending(t => t.CreatedAt).Take(top).ToListAsync(ct);
    }
}