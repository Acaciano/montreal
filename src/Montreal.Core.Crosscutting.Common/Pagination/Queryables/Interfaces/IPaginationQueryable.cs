using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Montreal.Core.Crosscutting.Common.Pagination
{
    public interface IPaginationQueryable<TEntity> : IOrderedQueryable<TEntity>, IEnumerable<TEntity>, IEnumerable, IOrderedQueryable, IQueryable, IQueryable<TEntity>
    {
        ICollection<PaginationConfiguratorObject<TEntity>> Configurations { get; set; }
        bool HasAllOrder { get; }
        bool HasAllFilter { get; }
        bool HasGlobalConfig { get; set; }
    }
}
