using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PositiveNewsPlatform.Application.Abstractions.Persistence;

namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly PositiveNewsDbContext _db;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(PositiveNewsDbContext db)
    {
        _db = db;
    }

    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        _transaction = await _db.Database.BeginTransactionAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        if (_transaction is null) return;
        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct)
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }
}