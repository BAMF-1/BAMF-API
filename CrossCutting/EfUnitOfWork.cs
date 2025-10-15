using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BAMF_API.Data;

namespace BAMF_API.CrossCutting;

public class EfUnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _ctx;
    private IDbContextTransaction? _currentTransaction;

    public EfUnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _ctx.SaveChangesAsync(ct);
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        if (_currentTransaction != null)
        {
            await action();
            return;
        }

        _currentTransaction = await _ctx.Database.BeginTransactionAsync(ct);
        try
        {
            await action();
            await _ctx.SaveChangesAsync(ct);
            await _currentTransaction.CommitAsync(ct);
        }
        catch
        {
            try { await _currentTransaction.RollbackAsync(ct); } catch { }
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _ctx.Dispose();
    }
}