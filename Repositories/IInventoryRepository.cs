using System;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Models;

namespace BAMF_API.Repositories;

public interface IInventoryRepository
{
    Task<Inventory?> GetByVariantIdAsync(Guid variantId, CancellationToken ct = default);
    Task AddAsync(Inventory inv, CancellationToken ct = default);
    void Update(Inventory inv);
}