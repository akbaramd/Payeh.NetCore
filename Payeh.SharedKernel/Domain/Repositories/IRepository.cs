using System.Linq.Expressions;

namespace Payeh.SharedKernel.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : Entity
{        
    
    public Task<IQueryable<TEntity>> GetQueryableAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> AddAsync(TEntity entity, bool autoSave = false);
    Task UpdateAsync(TEntity entity, bool autoSave = false);
    Task DeleteAsync(TEntity entity, bool autoSave = false);
    // Bulk operations
    Task AddRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
    
}