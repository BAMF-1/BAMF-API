
using BAMF_API.Data;
using BAMF_API.DTOs.Requests;
using BAMF_API.DTOs.Requests.AdminDashDTOs;
using BAMF_API.Extensions;
using BAMF_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BAMF_API.Controllers.Admin;

[ApiController]
[Route("api/admin/variants")]
[Authorize(Roles = "Admin")]
public class VariantsAdminController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public VariantsAdminController(ApplicationDbContext db) { _db = db; }

    [HttpGet("by-group/{groupId:guid}")]
    public async Task<IActionResult> ByGroup(Guid groupId, int page, CancellationToken ct)
    {
        var v = await _db.Variants
            .Where(x => x.ProductGroupId == groupId)
            .Select(x => new VariantDto
            {
                Id = x.Id,
                Sku = x.Sku,
                Color = x.Color,
                Size = x.Size,
                Price = x.Price,
                InventoryQuantity = x.Inventory.Quantity
            })
            .Paginate(page)
            .ToListAsync(ct);

        return Ok(v);
    }

    [HttpGet("by-group/count")]
    public async Task<IActionResult> CountByGroup(Guid groupId, CancellationToken ct)
    {
        var count = await _db.Variants.CountAsync(x => x.ProductGroupId == groupId, ct);
        return Ok(new { Count = count });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVariantRequest req, CancellationToken ct)
    {
        var group = await _db.ProductGroups.FindAsync(new object?[] { req.ProductGroupId }, ct);
        if (group == null) return BadRequest("Group not found.");
        if (await _db.Variants.AnyAsync(v => v.Sku == req.Sku, ct)) return Conflict("SKU exists.");

        var v = new Variant
        {
            Sku = req.Sku,
            ProductGroupId = req.ProductGroupId,
            Color = req.Color,
            Size = req.Size,
            Price = req.Price
        };
        _db.Variants.Add(v);
        await _db.SaveChangesAsync(ct);

        _db.Inventories.Add(new Inventory { VariantId = v.Id, Quantity = 0, LowStockThreshold = 0 });
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(ByGroup), new { groupId = v.ProductGroupId }, v);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVariantRequest req, CancellationToken ct)
    {
        var v = await _db.Variants.FindAsync(new object?[] { id }, ct);
        if (v == null) return NotFound();
        v.Color = req.Color;
        v.Size = req.Size;
        v.Price = req.Price;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
    {
        var v = await _db.Variants.FindAsync(new object?[] { id }, ct);
        if (v == null) return NotFound();
        v.IsDeleted = TrueFalse(True: true);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/inventory/adjust")]
    public async Task<IActionResult> AdjustInventory(Guid id, [FromBody] AdjustInventoryRequest req, CancellationToken ct)
    {
        var v = await _db.Variants.Include(x => x.Inventory).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (v == null) return NotFound();
        v.Inventory.Quantity += req.Delta;
        if (req.TransactionType == InventoryTransactionType.Restock && req.Delta > 0)
            v.Inventory.LastRestockDate = DateTimeOffset.UtcNow;

        _db.InventoryTransactions.Add(new InventoryTransaction
        {
            InventoryId = v.Inventory.Id,
            TransactionType = req.TransactionType,
            QuantityChange = req.Delta,
            ResultingQuantity = v.Inventory.Quantity,
            ReferenceId = req.ReferenceId
        });
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static bool TrueFalse(bool True = false) => True;
}
