using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Common.Data;
using Montreal.Core.Crosscutting.Common.Data;
using Montreal.Core.Crosscutting.Common.Entity.Base.V1;

namespace Montreal.Core.Crosscutting.Domain.Interfaces.Repositories.V1
{
    public interface ISqlServerRepository<TEntity> : IDisposable where TEntity : class, IEntity, new()
    {
        Task<IQueryable<TEntity>> ListAllAsync();
        Task<IList<TEntity>> ListAllAsync(params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IList<TEntity>> ListByExpressionAsync(Expression<Func<TEntity, bool>> expression);
        Task<IList<TEntity>> ListByExpressionAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetByIdAsync(Guid id);
        Task<TEntity> GetSingleByExpressionAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> GetSingleByExpressionAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetFirstByExpressionAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> GetFirstByExpressionAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<PagedList<TEntity>> GetAllPagedAsync(Order order, Page page,
            Expression<Func<TEntity, bool>> filter = null);
        Task<bool> ExistsByExpressionAsync(Expression<Func<TEntity, bool>> expression);
        Task<long> CountAllAsync();
        Task<long> CountByExpressionAsync(Expression<Func<TEntity, bool>> expression);
        Task DeleteByIdAsync(Guid id);
        Task DeleteByExpressionAsync(Expression<Func<TEntity, bool>> expression);
        Task InsertOrUpdateAsync(TEntity entity, params string[] ignorePropertiesUpdate);
        Task InsertRangeAsync(IList<TEntity> entities);
        Task<int> SaveChangesAsync();
    }
}