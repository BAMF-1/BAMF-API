using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Data;
using BAMF_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BAMF_API.Repositories;

public class ProductGroupRepository : IProductGroupRepository
{
    private readonly AppDbContext _ctx;
    public ProductGroupRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<bool> ExistsByObjectIdAsync(string objectId, CancellationToken ct = default)
    {
        return await _ctx.ProductGroups.AnyAsync(pg => pg.ObjectId == objectId, ct);
    }

    public async Task<ProductGroup?> GetBySlugOrObjectIdAsync(string idOrSlug, CancellationToken ct = default)
    {
        return await _ctx.ProductGroups.FirstOrDefaultAsync(pg => pg.Slug == idOrSlug || pg.ObjectId == idOrSlug, ct);
    }
}