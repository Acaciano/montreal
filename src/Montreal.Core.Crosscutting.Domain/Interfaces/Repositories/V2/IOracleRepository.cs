using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Common.Entity.Base.V2;

namespace Montreal.Core.Crosscutting.Domain.Interfaces.Repositories.V2
{
    public interface IOracleRepository<TEntity> : IDisposable where TEntity : class, IEntity
    {
        Task<IQueryable<TEntity>> ListAllAsync(bool track = true);
        Task<TEntity> GetByIdAsync(Guid id);
        Task<long> CountAllAsync();
        Task DeleteByIdAsync(Guid id);
        Task InsertOrUpdateAsync(TEntity entity, params string[] ignorePropertiesUpdate);
        Task InsertRangeAsync(IList<TEntity> entities);
        Task<int> SaveChangesAsync();
    }
}