using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Montreal.Core.Crosscutting.Common.Pagination
{
    public sealed class PaginationConfiguratorObject<TEntity>
    {
        public PaginationConfiguratorObject()
        {
            OrderExpressions = new List<OrderExpression<TEntity>>();
            FilterExpression = new List<Expression<Func<TEntity, object>>>();
        }

        public Type Type { get; set; }
        public string Property { get; set; }
        public ICollection<Expression<Func<TEntity, object>>> FilterExpression { get; set; }
        public ICollection<OrderExpression<TEntity>> OrderExpressions { get; set; }
        public Expression<Func<TEntity, object>> SelectExpression { get; set; }
    }

    public class OrderExpression<TEntity>
    {
        public bool? AscendingOrder { get; set; }
        public Expression<Func<TEntity, object>> PropertyOrderExpression { get; set; }
        public Expression<Func<TEntity, bool>> BoolOrderExpression { get; set; }
    }

}
