using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Models;

namespace BAMF_API.Repositories;

public interface IVariantRepository
{
    Task<Variant?> GetBySkuAsync(string sku, CancellationToken ct = default);
    Task<List<Variant>> ListByGroupAsync(Guid productGroupId, CancellationToken ct = default);
    Task AddAsync(Variant variant, CancellationToken ct = default);
    void Update(Variant variant);
}