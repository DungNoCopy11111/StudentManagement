using NHibernate;
using StudentMgmt.Application.Interfaces.Common;

namespace StudentMgmt.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ISession _session;
    private ITransaction? _transaction;

    public UnitOfWork(ISession session)
    {
        _session = session;
    }

    public Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction == null || !_transaction.IsActive)
        {
            _transaction = _session.BeginTransaction();
        }
        return Task.CompletedTask;
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction == null || !_transaction.IsActive)
            throw new InvalidOperationException("No active transaction to commit.");

        await _transaction.CommitAsync(ct);
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null && _transaction.IsActive)
        {
            await _transaction.RollbackAsync(ct);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            // 1. Đẩy dữ liệu xuống DB
            await _session.FlushAsync(ct);

            // 2. Chốt hạ Transaction
            if (_transaction != null && _transaction.IsActive)
            {
                await _transaction.CommitAsync(ct);
            }

            return 1;
        }
        catch (Exception)
        {
            if (_transaction != null && _transaction.IsActive)
            {
                await _transaction.RollbackAsync(ct);
            }
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}