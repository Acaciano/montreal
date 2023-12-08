using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Common.Entity.Base.V2;
using Montreal.Core.Crosscutting.Domain.Interfaces.Repositories.V2;

namespace Montreal.Core.Crosscutting.Infrastructure.Repositories.V2
{
    public abstract class OracleRepository<TEntity> : IOracleRepository<TEntity> where TEntity : BaseEntity, IEntity
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

        public async Task<IQueryable<TEntity>> ListAllAsync(bool track = true)
        {
            IQueryable<TEntity> query = null;

            if (this.DbSet().Any())
            {
                query = await Task.Run(() => DbSet().AsQueryable());

                if (!track)
                {
                    query = query.AsNoTracking();
                }
            }

            return query;
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await DbSet().FindAsync(id);
        }

        public async Task<long> CountAllAsync()
        {
            return await DbSet().AnyAsync() ? await DbSet().AsNoTracking().CountAsync() : 0;
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