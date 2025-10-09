
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BAMF_API.Data;
using BAMF_API.DTOs.Responses;
using System.Linq;

namespace BAMF_API.Controllers.Public;

[ApiController]
[Route("groups")]
public class PublicGroupsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PublicGroupsController(ApplicationDbContext db) { _db = db; }

    // GET /groups/{idOrSlug}
    [HttpGet("{idOrSlug}")]
    public async Task<IActionResult> GetGroup(string idOrSlug, [FromQuery] string? sku, [FromQuery] string? color, [FromQuery] string? size,
        [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
        [FromQuery] string? sort = "price", [FromQuery] string? dir = "asc",
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var group = await _db.ProductGroups
            .Include(g => g.Category)
            .FirstOrDefaultAsync(g => (g.Slug == idOrSlug) || (g.ObjectId == idOrSlug), ct);

        if (group == null) return NotFound();

        var variants = _db.Variants
            .Include(v => v.Inventory)
            .Include(v => v.VariantImages)
            .Where(v => v.ProductGroupId == group.Id && !v.IsDeleted);

        var anyActive = await variants.AnyAsync(ct);
        if (!anyActive) return NotFound(); // public policy

        if (!string.IsNullOrWhiteSpace(color)) variants = variants.Where(v => v.Color == color);
        if (!string.IsNullOrWhiteSpace(size)) variants = variants.Where(v => v.Size == size);
        if (minPrice.HasValue) variants = variants.Where(v => v.Price >= minPrice.Value);
        if (maxPrice.HasValue) variants = variants.Where(v => v.Price <= maxPrice.Value);

        var dirVal = (dir ?? "asc").ToLower() == "desc" ? -1 : 1;
        variants = (sort ?? "price").ToLower() switch
        {
            "size" => dirVal == 1 ? variants.OrderBy(v => v.Size) : variants.OrderByDescending(v => v.Size),
            "color" => dirVal == 1 ? variants.OrderBy(v => v.Color) : variants.OrderByDescending(v => v.Color),
            _ => dirVal == 1 ? variants.OrderBy(v => v.Price) : variants.OrderByDescending(v => v.Price),
        };

        var total = await variants.CountAsync(ct);
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var list = await variants.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        string ResolvePrimary(BAMF_API.Models.Variant v)
        {
            var skuPrimary = v.VariantImages.Where(i => i.IsPrimary).OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault();
            if (!string.IsNullOrEmpty(skuPrimary)) return skuPrimary;
            var colorPrimary = _db.ColorImages.Where(ci => ci.ProductGroupId == v.ProductGroupId && ci.Color == v.Color && ci.IsPrimary)
                                              .OrderBy(ci => ci.SortOrder).Select(ci => ci.Url).FirstOrDefault();
            return colorPrimary;
        }

        var globalMin = await _db.Variants.Where(v => v.ProductGroupId == group.Id && !v.IsDeleted).MinAsync(v => v.Price, ct);
        var globalMax = await _db.Variants.Where(v => v.ProductGroupId == group.Id && !v.IsDeleted).MaxAsync(v => v.Price, ct);

        var resp = new GroupPageResponse
        {
            ObjectId = group.ObjectId,
            Name = group.Name,
            MainCategory = group.Category.Name,
            HeroImageUrl = list.Select(ResolvePrimary).FirstOrDefault() ?? await _db.ColorImages
                .Where(ci => ci.ProductGroupId == group.Id && ci.IsPrimary)
                .OrderBy(ci => ci.SortOrder).Select(ci => ci.Url).FirstOrDefaultAsync(ct),
            MinPrice = globalMin,
            MaxPrice = globalMax,
            InStockAny = await _db.Inventories.AnyAsync(i => _db.Variants.Any(v => v.Id == i.VariantId && v.ProductGroupId == group.Id && !v.IsDeleted) && i.Quantity > 0, ct),
            Page = page,
            PageSize = pageSize,
            TotalVariants = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Variants = list.Select(v => new VariantItem
            {
                Sku = v.Sku,
                Color = v.Color,
                Size = v.Size,
                Price = v.Price,
                InStock = v.Inventory.Quantity > 0,
                PrimaryImageUrl = ResolvePrimary(v)
            }).ToList()
        };

        // facets
        var groupActive = _db.Variants.Where(v => v.ProductGroupId == group.Id && !v.IsDeleted);
        resp.Colors = await groupActive.GroupBy(v => v.Color).Select(g => new FacetItem { Value = g.Key, Count = g.Count() }).ToListAsync(ct);
        resp.Sizes = await groupActive.GroupBy(v => v.Size).Select(g => new FacetItem { Value = g.Key, Count = g.Count() }).ToListAsync(ct);
        resp.PriceFacet = new PriceFacet { GlobalMax = globalMax, GlobalMin = globalMin };

        return Ok(resp);
    }
}
