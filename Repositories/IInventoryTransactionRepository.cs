using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Models;

namespace BAMF_API.Repositories;

public interface IInventoryTransactionRepository
{
    Task AddAsync(InventoryTransaction tx, CancellationToken ct = default);
    Task<IReadOnlyList<InventoryTransaction>> ListByInventoryAsync(Guid inventoryId, int top = 100, CancellationToken ct = default);
}