
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BAMF_API.Data;
using BAMF_API.DTOs.Requests;
using BAMF_API.DTOs.Responses;
using System.Linq;

namespace BAMF_API.Controllers.Public;

[ApiController]
[Route("api/products")]
public class PublicProductsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly LinkGenerator _links;

    public PublicProductsController(ApplicationDbContext db, LinkGenerator links)
    {
        _db = db;
        _links = links;
    }

    // SKU-first listing
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SkuListItemResponse>), 200)]
    public async Task<IActionResult> List([FromQuery] PublicListQuery q, CancellationToken ct)
    {
        var variants = _db.Variants
            .Include(v => v.ProductGroup).ThenInclude(pg => pg.Category)
            .Include(v => v.Inventory)
            .Include(v => v.VariantImages)
            .Where(v => !v.IsDeleted && !v.ProductGroup.IsDeleted);

        if (!string.IsNullOrWhiteSpace(q.CategorySlug))
            variants = variants.Where(v => v.ProductGroup.Category.Slug == q.CategorySlug);

        if (!string.IsNullOrWhiteSpace(q.Color))
            variants = variants.Where(v => v.Color == q.Color);

        if (!string.IsNullOrWhiteSpace(q.Size))
            variants = variants.Where(v => v.Size == q.Size);

        if (q.MinPrice.HasValue) variants = variants.Where(v => v.Price >= q.MinPrice.Value);
        if (q.MaxPrice.HasValue) variants = variants.Where(v => v.Price <= q.MaxPrice.Value);

        // Sort
        var dir = (q.Dir ?? "asc").ToLower() == "desc" ? -1 : 1;
        variants = (q.Sort ?? "price").ToLower() switch
        {
            "size" => dir == 1 ? variants.OrderBy(v => v.Size) : variants.OrderByDescending(v => v.Size),
            "color" => dir == 1 ? variants.OrderBy(v => v.Color) : variants.OrderByDescending(v => v.Color),
            _ => dir == 1 ? variants.OrderBy(v => v.Price) : variants.OrderByDescending(v => v.Price),
        };

        var page = Math.Max(1, q.Page);
        var pageSize = Math.Clamp(q.PageSize, 1, 100);

        var total = await variants.CountAsync(ct);
        var items = await variants
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // Resolve primary image (SKU override or color-level)
        string ResolvePrimary(BAMF_API.Models.Variant v)
        {
            var skuPrimary = v.VariantImages.Where(i => i.IsPrimary).OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault();
            if (!string.IsNullOrEmpty(skuPrimary)) return skuPrimary;
            var colorPrimary = _db.ColorImages.Where(ci => ci.ProductGroupId == v.ProductGroupId && ci.Color == v.Color && ci.IsPrimary)
                                              .OrderBy(ci => ci.SortOrder).Select(ci => ci.Url).FirstOrDefault();
            return colorPrimary;
        }

        var responses = new List<SkuListItemResponse>();
        foreach (var v in items)
        {
            var primary = ResolvePrimary(v);
            var slugOrObject = string.IsNullOrWhiteSpace(v.ProductGroup.Slug) ? v.ProductGroup.ObjectId : v.ProductGroup.Slug!;
            var groupPath = $"/groups/{slugOrObject}?sku={Uri.EscapeDataString(v.Sku)}";
            var siblingsCount = await _db.Variants.CountAsync(x => x.ProductGroupId == v.ProductGroupId && !x.IsDeleted && x.Sku != v.Sku, ct);

            responses.Add(new SkuListItemResponse
            {
                Sku = v.Sku,
                ObjectId = v.ProductGroup.ObjectId,
                GroupName = v.ProductGroup.Name,
                MainCategory = v.ProductGroup.Category.Name,
                Color = v.Color,
                Size = v.Size,
                Price = v.Price,
                InStock = v.Inventory.Quantity > 0,
                PrimaryImageUrl = primary,
                GroupLink = groupPath,
                MoreVariantsCount = Math.Max(0, siblingsCount)
            });
        }

        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(responses);
    }
}
