using System;
using System.Linq.Expressions;

namespace Montreal.Core.Crosscutting.Common.Pagination
{
    public interface IPaginationConfigQueryable<TEntity> : IPaginationQueryable<TEntity>
    {

        /// <summary>
        /// Associate a navigation property to order with the literal name.
        /// </summary>
        /// <param name="property">Navigation property to order</param>
        /// <param name="isAscending">Sorts ascedning or descending</param>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing all configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IPaginationConfigQueryable<TEntity> AddOrder(Expression<Func<TEntity, object>> property, bool? isAscending = null);

        /// <summary>
        /// Associate a bool expression of the type of entity being queried (TEntity) with the literal name.
        /// </summary>
        /// <param name="boolExpression">Bool expression to order</param>
        /// <param name="isAscending">Sorts ascedning or descending</param>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing all configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IPaginationConfigQueryable<TEntity> AddOrder(Expression<Func<TEntity, bool>> boolExpression, bool? isAscending = null);

        /// <summary>
        /// Associate a navigation property to filter with the literal name.
        /// </summary>
        /// <param name="property">Navigation property to filter</param>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing all configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IPaginationConfigQueryable<TEntity> AddFilter(Expression<Func<TEntity, object>> property);

        /// <summary>
        /// Associate a navigation property to order and filter with the literal name.
        /// </summary>
        /// <param name="property">Navigation property to order and filter</param>
        /// <param name="isAscending">Sorts ascedning or descending</param>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing all configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        IPaginationConfigQueryable<TEntity> AddFilterAndOrder(Expression<Func<TEntity, object>> property, bool? isAscending = null);
    }
}
