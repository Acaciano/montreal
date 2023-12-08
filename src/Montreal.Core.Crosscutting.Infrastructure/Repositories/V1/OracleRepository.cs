using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Common.Entity.Base.V1;
using Montreal.Core.Crosscutting.Common.Extensions;
using Montreal.Core.Crosscutting.Domain.Interfaces.Repositories.V1;

namespace Montreal.Core.Crosscutting.Infrastructure.Repositories.V1
{
    public abstract class OracleRepository<TEntity> : IOracleRepository<TEntity> where TEntity : BaseEntity, IEntity, new()
    {
        public DbContext Context;

        protected OracleRepository() { }

        protected OracleRepository(DbContext context) => Context = context;

        ~OracleRepository()
        {
            Dispose(false);
        }

        public DbSet<TEntity> DbSet()
        {
            return Context.Set<TEntity>();
        }

        public async Task<IQueryable<TEntity>> ListAllAsync()
        {
            return DbSet().Any() ? await Task.Run(() => DbSet().AsQueryable().AsNoTracking()) : null;
        }

        public async Task<IList<TEntity>> ListAllAsync(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return DbSet().Any() ? await DbSet().IncludeProperties(includeProperties).ToListAsync() : null;
        }

        public async Task<IList<TEntity>> ListByExpressionAsync(Expression<Func<TEntity, bool>> expression)
        {
            return DbSet().Any(expression) ? await DbSet().Where(expression).ToListAsync() : null;
        }

        public async Task<IList<TEntity>> ListByExpressionAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return DbSet().Any(expression) ? await DbSet().Where(expression).IncludeProperties(includeProperties).ToListAsync() : null;
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await DbSet().FindAsync(id);
        }

        public async Task<ICollection<TEntity>> GetByIdAsync(params Guid[] ids)
        {
            return await DbSet().AsNoTracking().Where(x => ids.Any(id => id == x.Id)).ToListAsync();
        }

        public async Task<TEntity> GetSingleByExpressionAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await DbSet().SingleOrDefaultAsync(expression);
        }

        public async Task<TEntity> GetSingleByExpressionAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return await DbSet().IncludeProperties(includeProperties).SingleOrDefaultAsync(expression);
        }

        public async Task<TEntity> GetFirstByExpressionAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await DbSet().FirstOrDefaultAsync(expression);
        }

        public async Task<TEntity> GetFirstByExpressionAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return await DbSet().IncludeProperties(includeProperties).FirstOrDefaultAsync(expression);
        }

        public async Task<bool> ExistsByExpressionAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await DbSet().AnyAsync(expression);
        }

        public async Task<long> CountAllAsync()
        {
            return await DbSet().AnyAsync() ? await DbSet().AsNoTracking().CountAsync() : 0;
        }

        public async Task<long> CountByExpressionAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await ExistsByExpressionAsync(expression) ? await DbSet().AsNoTracking().CountAsync(expression) : 0;
        }

        public async Task InsertOrUpdateAsync(TEntity entity, params string[] ignorePropertiesUpdate)
        {
            var context = await GetByIdAsync(entity.Id);

            if (context == null)
                await DbSet().AddAsync(entity);
            else
            {
                Context.Entry(context).CurrentValues.SetValues(entity);
                Context.Entry(context).State = EntityState.Modified;

                if (ignorePropertiesUpdate != null && ignorePropertiesUpdate.Length > 0)
                {
                    foreach (var p in ignorePropertiesUpdate)
                    {
                        if (entity.GetType().GetProperties().Any(i => i.Name == p))
                        {
                            Context.Entry(context).Property(p).IsModified = false;
                        }
                    }
                }
            }
        }

        public async Task InsertRangeAsync(IList<TEntity> entities)
        {
            await DbSet().AddRangeAsync(entities);
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var context = await GetByIdAsync(id);

            if (context == null) return;
            Context.Entry(context).State = EntityState.Deleted;

            DbSet().Remove(context);
        }

        public async Task DeleteByExpressionAsync(Expression<Func<TEntity, bool>> expression)
        {
            var items = await ListByExpressionAsync(expression);

            if (items.Count > 0)
            {
                Context.Entry(items).State = EntityState.Deleted;
                DbSet().RemoveRange(items);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Context == null) return;

            Context.Dispose();
            Context = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}