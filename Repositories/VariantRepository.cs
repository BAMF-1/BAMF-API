// BAMF_API/Repositories/VariantRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BAMF_API.Interfaces.ProductInterfaces;
using BAMF_API.Models;
using BAMF_API.Data; // Your existing DbContext namespace

namespace BAMF_API.Repositories
{
    public class VariantRepository : IVariantRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VariantRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create a new variant
        public async Task AddAsync(Variant variant, CancellationToken ct = default)
        {
            await _dbContext.Variants.AddAsync(variant, ct);
        }

        // Update an existing variant
        public void Update(Variant variant)
        {
            _dbContext.Variants.Update(variant);
        }

        // Get a variant by SKU (active variants only)
        public async Task<Variant?> GetBySkuAsync(string sku, CancellationToken ct = default)
        {
            return await _dbContext.Variants
                                   .AsNoTracking()               // read-only tracking
                                   .FirstOrDefaultAsync(
                                       v => v.Sku == sku && !v.IsDeleted,
                                       ct
                                   );
        }

        // Get a variant by SKU with its Inventory attached, if you need it
        public async Task<Variant?> GetBySkuWithInventoryAsync(string sku, CancellationToken ct = default)
        {
            return await _dbContext.Variants
                                   .Include(v => v.Inventory)
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(
                                       v => v.Sku == sku && !v.IsDeleted,
                                       ct
                                   );
        }

        // List all active variants in a product group
        public async Task<List<Variant>> ListByGroupAsync(Guid productGroupId, CancellationToken ct = default)
        {
            return await _dbContext.Variants
                                   .AsNoTracking()
                                   .Where(v => v.ProductGroupId == productGroupId && !v.IsDeleted)
                                   .ToListAsync(ct);
        }

        // Optional: admin-only methods to include deleted variants
        public async Task<Variant?> GetBySkuIncludingDeletedAsync(string sku, CancellationToken ct = default)
        {
            return await _dbContext.Variants
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(v => v.Sku == sku, ct);
        }

        public async Task<List<Variant>> ListByGroupIncludingDeletedAsync(Guid productGroupId, CancellationToken ct = default)
        {
            return await _dbContext.Variants
                                   .AsNoTracking()
                                   .Where(v => v.ProductGroupId == productGroupId)
                                   .ToListAsync(ct);
        }
    }
}

