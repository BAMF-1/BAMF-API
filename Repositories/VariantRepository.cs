using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Data;
using BAMF_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BAMF_API.Repositories;

public class VariantRepository : IVariantRepository
{
    private readonly AppDbContext _ctx;
    public VariantRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Variant variant, CancellationToken ct = default)
    {
        await _ctx.Variants.AddAsync(variant, ct);
    }

    public async Task<Variant?> GetBySkuAsync(string sku, CancellationToken ct = default)
    {
        return await _ctx.Variants.Include(v => v.Inventory).FirstOrDefaultAsync(v => v.Sku == sku, ct);
    }

    public async Task<List<Variant>> ListByGroupAsync(Guid productGroupId, CancellationToken ct = default)
    {
        return await _ctx.Variants.Where(v => v.ProductGroupId == productGroupId && !v.IsDeleted).ToListAsync(ct);
    }

    public void Update(Variant variant)
    {
        _ctx.Variants.Update(variant);
    }
}