// BAMF_API/Repositories/InventoryTransactionRepository.cs
using BAMF_API.Data; // adjust as needed
using BAMF_API.Interfaces.InventoryInterfaces;
using BAMF_API.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BAMF_API.Repositories
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public InventoryTransactionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(InventoryTransaction tx, CancellationToken ct = default)
        {
            await _dbContext.InventoryTransactions.AddAsync(tx, ct);
        }
    }
}
