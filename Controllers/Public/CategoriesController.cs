
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BAMF_API.Data;
using BAMF_API.DTOs.Responses;

namespace BAMF_API.Controllers.Public;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IEnumerable<CategoryResponse>> Get(CancellationToken ct)
    {
        var list = await _db.Categories.OrderBy(c => c.Name).ToListAsync(ct);
        return list.Select(c => new CategoryResponse { Id = c.Id, Name = c.Name, Slug = c.Slug });
    }
}
