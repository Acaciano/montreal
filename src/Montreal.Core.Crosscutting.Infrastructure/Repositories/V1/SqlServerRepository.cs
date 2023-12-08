using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Common.Data;
using Montreal.Core.Crosscutting.Common.Entity.Base.V1;
using Montreal.Core.Crosscutting.Common.Extensions;
using Montreal.Core.Crosscutting.Common.Extensions.V1;
using Montreal.Core.Crosscutting.Domain.Interfaces.Repositories.V1;

namespace Montreal.Core.Crosscutting.Infrastructure.Repositories.V1
{
    public abstract class SqlServerRepository<TEntity> : ISqlServerRepository<TEntity> where TEntity : BaseEntity, IEntity, new()
    {
        public DbContext Context;

        protected SqlServerRepository() { }

        protected SqlServerRepository(DbContext context)
        {
            Context = context;
        }

        ~SqlServerRepository()
        {
            Dispose(false);
        }

        public DbSet<TEntity> DbSet()
        {
            return Context.Set<TEntity>();
        }

        public DbSet<TEntity> VerifyDataIsDuplicated(string tableName, string columnName, string valueCompare)
        {
            var stringCleaned = valueCompare.GetStringWithoutAccentsSpecialCharacters();

            var stringQuery = string.Concat("SELECT * FROM ", tableName, " WHERE ", columnName, " = '", stringCleaned, " '");


            var result = Context.Set<TEntity>().FromSqlRaw(stringQuery);

            return (DbSet<TEntity>)result;
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

        public async Task<PagedList<TEntity>> GetAllPagedAsync(Order order, Page page,
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> cursor;

            if (filter != null)
            {
                cursor = order.Crescent
                ? Context.Set<TEntity>().Where(filter).OrderByDescending(x => x.CreatedAt).AsQueryable()
                : Context.Set<TEntity>().Where(filter).OrderBy(x => x.CreatedAt).AsQueryable();
            }
            else
            {
                cursor = order.Crescent
                ? Context.Set<TEntity>().OrderByDescending(x => x.CreatedAt).AsQueryable()
                : Context.Set<TEntity>().OrderBy(x => x.CreatedAt).AsQueryable();
            }

            int count = cursor.Count();
            var result = await cursor.Skip((page.Index - 1) * page.Quantity).Take(page.Quantity).ToListAsync();

            var pagedResult = new PagedList<TEntity>();
            pagedResult.TotalRecords = count;
            pagedResult.CurrentPage = page.Index;
            pagedResult.PageSize = page.Quantity;
            pagedResult.TotalPages = (int)Math.Ceiling(pagedResult.TotalRecords / (double)page.Quantity);
            pagedResult.Results = result;

            return pagedResult;
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

                if (ignorePropertiesUpdate != null && ignorePropertiesUpdate.Count() > 0)
                {
                    foreach (var p in ignorePropertiesUpdate)
                    {
                        if (entity.GetType().GetProperties().Count(i => i.Name == p) > 0)
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