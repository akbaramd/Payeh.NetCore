using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Payeh.SharedKernel.Domain;
using Payeh.SharedKernel.Domain.Repositories;

namespace Payeh.SharedKernel.EntityFrameworkCore.Domain;

public class EntityFrameworkRepository<TEntity,TDbContext>: IRepository<TEntity> where TEntity : Entity where TDbContext : DbContext
    {
        private readonly TDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public EntityFrameworkRepository(TDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return await Task.FromResult(_dbSet.AsQueryable());
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await FindOneAsync(predicate);
            if (entity == null)
                throw new InvalidOperationException("Entity not found.");

            return entity;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<TEntity> AddAsync(TEntity entity, bool autoSave = false)
        {
            await _dbSet.AddAsync(entity);
            if (autoSave)
            {
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public async Task UpdateAsync(TEntity entity, bool autoSave = false)
        {
            _dbSet.Update(entity);
            if (autoSave)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(TEntity entity, bool autoSave = false)
        {
            _dbSet.Remove(entity);
            if (autoSave)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            await _dbSet.AddRangeAsync(entities);
            if (autoSave)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            _dbSet.UpdateRange(entities);
            if (autoSave)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            _dbSet.RemoveRange(entities);
            if (autoSave)
            {
                await _context.SaveChangesAsync();
            }
        }
    }