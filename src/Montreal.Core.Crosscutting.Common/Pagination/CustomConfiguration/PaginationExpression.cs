using System;
using System.Linq.Expressions;

namespace Montreal.Core.Crosscutting.Common.Pagination
{
    public sealed class PaginationExpression<TEntity>
    {
        internal PaginationExpression() { }

        internal PaginationExpression(PaginationConfiguratorObject<TEntity> configuratorObject) => _configuratorObject = configuratorObject;

        private readonly PaginationConfiguratorObject<TEntity> _configuratorObject;

        public PaginationExpression<TEntity> AddOrder(Expression<Func<TEntity, bool>> orderExpression, bool? ascendingOrder = null)
        {
            this._configuratorObject.OrderExpressions.Add(new OrderExpression<TEntity>() { BoolOrderExpression = orderExpression, AscendingOrder = ascendingOrder });

            return this;
        }

        public PaginationExpression<TEntity> AddOrder(Expression<Func<TEntity, object>> orderExpression, bool? ascendingOrder = null)
        {
            this._configuratorObject.OrderExpressions.Add(new OrderExpression<TEntity>() { PropertyOrderExpression = orderExpression, AscendingOrder = ascendingOrder });

            return this;
        }

        public PaginationExpression<TEntity> AddFilter(Expression<Func<TEntity, object>> filter)
        {
            this._configuratorObject.FilterExpression.Add(filter);

            return this;
        }
    }
}
