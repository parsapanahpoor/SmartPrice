using Microsoft.EntityFrameworkCore;
using SmartPrice.Application.Interfaces;
using SmartPrice.Infrastructure.Data;
using System.Linq.Expressions;

namespace SmartPrice.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }

    public async Task<List<T>> ListAsync(CancellationToken ct)
    {
        return await _dbSet.ToListAsync(ct);
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, ct);
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
    {
        return await _dbSet.Where(predicate).ToListAsync(ct);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
    {
        return await _dbSet.CountAsync(predicate, ct);
    }
}
