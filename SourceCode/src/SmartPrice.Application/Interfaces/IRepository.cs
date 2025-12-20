using SmartPrice.Domain.Entities;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get an entity by its identifier
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all entities</returns>
    Task<List<T>> ListAsync(CancellationToken ct);

    /// <summary>
    /// Add a new entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Added entity</returns>
    Task<T> AddAsync(T entity, CancellationToken ct);

    /// <summary>
    /// Update an existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateAsync(T entity, CancellationToken ct);

    /// <summary>
    /// Delete an entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    /// <param name="ct">Cancellation token</param>
    Task DeleteAsync(T entity, CancellationToken ct);

    /// <summary>
    /// Get first entity matching a condition or null
    /// </summary>
    /// <param name="predicate">Condition to match</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>First matching entity or null</returns>
    Task<T?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken ct);

    /// <summary>
    /// Get all entities matching a condition
    /// </summary>
    /// <param name="predicate">Condition to match</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of matching entities</returns>
    Task<List<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken ct);

    /// <summary>
    /// Count entities matching a condition
    /// </summary>
    /// <param name="predicate">Condition to match</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Count of matching entities</returns>
    Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken ct);
}
