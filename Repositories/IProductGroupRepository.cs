using System;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Models;

namespace BAMF_API.Repositories;

public interface IProductGroupRepository
{
    Task<ProductGroup?> GetBySlugOrObjectIdAsync(string idOrSlug, CancellationToken ct = default);
    Task<bool> ExistsByObjectIdAsync(string objectId, CancellationToken ct = default);
}