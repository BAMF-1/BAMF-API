
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BAMF_API.Data;
using BAMF_API.DTOs.Requests;
using BAMF_API.Models;

namespace BAMF_API.Controllers.Admin;

[ApiController]
[Route("api/admin/groups")]
[Authorize(Roles = "Admin")]
public class GroupsAdminController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public GroupsAdminController(ApplicationDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _db.ProductGroups.Include(g => g.Category).ToListAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest req, CancellationToken ct)
    {
        if (await _db.ProductGroups.AnyAsync(g => g.ObjectId == req.ObjectId, ct))
            return Conflict("ObjectId already exists.");

        var cat = await _db.Categories.FindAsync(new object?[] { req.CategoryId }, ct);
        if (cat == null) return BadRequest("Category not found.");

        var group = new ProductGroup
        {
            ObjectId = req.ObjectId,
            Name = req.Name,
            CategoryId = req.CategoryId,
            Slug = GenerateSlug(req.Name)
        };
        _db.ProductGroups.Add(group);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetAll), new { id = group.Id }, group);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupRequest req, CancellationToken ct)
    {
        var group = await _db.ProductGroups.FindAsync(new object?[] { id }, ct);
        if (group == null) return NotFound();
        group.Name = req.Name;
        group.CategoryId = req.CategoryId;
        group.Slug = string.IsNullOrWhiteSpace(req.Slug) ? GenerateSlug(req.Name) : req.Slug;
        group.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var group = await _db.ProductGroups.Include(g => g.Variants).FirstOrDefaultAsync(g => g.Id == id, ct);
        if (group == null) return NotFound();
        if (group.Variants.Any(v => !v.IsDeleted)) return BadRequest("Delete or soft-delete variants first.");
        group.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static string GenerateSlug(string input)
    {
        var s = new string(input.ToLowerInvariant().Where(c => char.IsLetterOrDigit(c) || c==' ').ToArray()).Replace(' ','-');
        return s;
    }
}
