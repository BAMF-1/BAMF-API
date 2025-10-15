using System;
using System.Threading;
using System.Threading.Tasks;

namespace BAMF_API.CrossCutting;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
}