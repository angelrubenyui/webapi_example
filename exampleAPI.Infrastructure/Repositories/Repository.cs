using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MiApi.Domain.Interfaces;
using MiApi.Infrastructure.Data;

namespace MiApi.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken: ct);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(ct);
    }

    public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return entity;
    }

    public virtual Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.FromResult(entity); // Retorna Task<T> de forma síncrona, cumpliendo la interfaz
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        _dbSet.Remove(entity);
        return true;
    }
    public virtual Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }
}