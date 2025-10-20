// BAMF_API/Repositories/InventoryRepository.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BAMF_API.Interfaces.InventoryInterfaces;
using BAMF_API.Models;
using BAMF_API.Data; // adjust namespace for your DbContext

namespace BAMF_API.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public InventoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Inventory?> GetByVariantIdAsync(Guid variantId, CancellationToken ct = default)
        {
            // Returns the inventory row for a given VariantId, if present.
            return await _dbContext.Inventories
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(i => i.VariantId == variantId, ct);
        }

        public async Task AddAsync(Inventory inventory, CancellationToken ct = default)
        {
            await _dbContext.Inventories.AddAsync(inventory, ct);
        }

        public void Update(Inventory inventory)
        {
            _dbContext.Inventories.Update(inventory);
        }
    }
}

