using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Montreal.Core.Crosscutting.Common.Pagination
{
    public class PaginationQueryable<TEntity> : IPaginationQueryable<TEntity>
    {
        public bool HasGlobalConfig { get; set; }
        public bool HasAllOrder { get; private set; }
        public bool HasAllFilter { get; private set; }
        public ICollection<PaginationConfiguratorObject<TEntity>> Configurations { get; set; }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public PaginationQueryable(IQueryable<TEntity> query,  ICollection<PaginationConfiguratorObject<TEntity>> configurations = null, 
            bool? hasAllorder = null, bool? hasAllFilter = null, bool? hasGlobalConfig = null)
        {
            ElementType = query.ElementType;
            Expression = query.Expression;
            Provider = query.Provider;

            Configurations = configurations ?? (query as PaginationQueryable<TEntity>)?.Configurations ?? new List<PaginationConfiguratorObject<TEntity>>();
            HasAllOrder = hasAllorder ?? (query as PaginationQueryable<TEntity>)?.HasAllOrder ?? false;
            HasAllFilter = hasAllFilter ?? (query as PaginationQueryable<TEntity>)?.HasAllFilter ?? false;
            HasGlobalConfig = hasGlobalConfig ?? (query as PaginationQueryable<TEntity>)?.HasGlobalConfig ?? false;
        }

        public IEnumerator<TEntity> GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return this;
        }
    }
}
