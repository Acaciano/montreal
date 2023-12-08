using System;
using System.Linq;
using System.Linq.Expressions;
using Montreal.Core.Crosscutting.Common.Extensions;

namespace Montreal.Core.Crosscutting.Common.Pagination
{
    public abstract class PaginationCustomConfiguration
    {
        /// <summary>
        /// Associate a navigation property to order with the literal name.
        /// </summary>
        /// <param name="property">Navigation property to order</param>
        /// <param name="isAscending">Sorts ascedning or descending</param>
        /// <remarks>To use this configuration, you need to call <see cref="QueryableExtensions.AllowGlobalPaginationConfig{TEntity}(IQueryable{TEntity}, bool)"/> method in your query.</remarks>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing all configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected PaginationExpression<TEntity> CreateGlobalOrder<TEntity>(string str, Expression<Func<TEntity, object>> orderProperty, bool? ascendingOrder = null)
        {
            var existingProperty = PaginationConfiguratorFactory<TEntity>.GetByProperty(str);

            if (existingProperty != null && existingProperty.OrderExpressions.Any())
            {
                existingProperty.OrderExpressions.Add(new OrderExpression<TEntity>() { PropertyOrderExpression = orderProperty, AscendingOrder = ascendingOrder });
                return new PaginationExpression<TEntity>(existingProperty);
            }

            var configuration = new PaginationConfiguratorObject<TEntity>();

            configuration.Property = str.ToLower();
            configuration.OrderExpressions.Add(new OrderExpression<TEntity>() { PropertyOrderExpression = orderProperty, AscendingOrder = ascendingOrder });
            configuration.Type = typeof(TEntity);

            return new PaginationExpression<TEntity>(PaginationConfiguratorFactory<TEntity>.AddConfiguration(configuration));
        }

        /// <summary>
        /// Associate a bool expression of the type of entity being queried (TEntity) with the literal name.
        /// </summary>
        /// <param name="boolExpression">Bool expression to order</param>
        /// <param name="isAscending">Sorts ascedning or descending</param>
        /// <remarks>To use this configuration, you need to call <see cref="QueryableExtensions.AllowGlobalPaginationConfig{TEntity}(IQueryable{TEntity}, bool)"/> method in your query.</remarks>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing all configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected PaginationExpression<TEntity> CreateGlobalOrder<TEntity>(string str, Expression<Func<TEntity, bool>> orderProperty, bool? ascendingOrder = null)
        {
            var existingProperty = PaginationConfiguratorFactory<TEntity>.GetByProperty(str);

            if (existingProperty != null && existingProperty.OrderExpressions != null)
            {
                existingProperty.OrderExpressions.Add(new OrderExpression<TEntity>() { BoolOrderExpression = orderProperty, AscendingOrder = ascendingOrder});
                return new PaginationExpression<TEntity>(existingProperty);
            }

            var configuration = new PaginationConfiguratorObject<TEntity>();

            configuration.Property = str.ToLower();
            configuration.OrderExpressions.Add(new OrderExpression<TEntity>() { BoolOrderExpression = orderProperty, AscendingOrder = ascendingOrder });
            configuration.Type = typeof(TEntity);

            return new PaginationExpression<TEntity>(PaginationConfiguratorFactory<TEntity>.AddConfiguration(configuration));
        }

        /// <summary>
        /// Associate a navigation property to filter with the literal name.
        /// </summary>
        /// <param name="property">Navigation property to filter</param>
        /// <remarks>To use this configuration, you need to call <see cref="QueryableExtensions.AllowGlobalPaginationConfig{TEntity}(IQueryable{TEntity}, bool)"/> method in your query.</remarks>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing all configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected PaginationExpression<TEntity> CreateGlobalFilter<TEntity>(string str, Expression<Func<TEntity, object>> filter)
        {
            var existingProperty = PaginationConfiguratorFactory<TEntity>.GetByProperty(str);

            if (existingProperty != null && existingProperty.FilterExpression != null)
            {
                existingProperty.FilterExpression.Add(filter);
                return new PaginationExpression<TEntity>(existingProperty);
            }

            var configuration = new PaginationConfiguratorObject<TEntity>();

            configuration.Property = str.ToLower();
            configuration.FilterExpression.Add(filter);
            configuration.Type = typeof(TEntity);

            return new PaginationExpression<TEntity>(PaginationConfiguratorFactory<TEntity>.AddConfiguration(configuration));
        }

        /// <summary>
        /// Associate a navigation property to order and filter with the literal name.
        /// </summary>
        /// <param name="property">Navigation property to order and filter</param>
        /// <param name="isAscending">Sorts ascedning or descending</param>
        /// <remarks>To use this configuration, you need to call <see cref="QueryableExtensions.AllowGlobalPaginationConfig{TEntity}(IQueryable{TEntity}, bool)"/> method in your query.</remarks>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing all configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected PaginationExpression<TEntity> CreateGlobalFilterAndOrder<TEntity>(string str, Expression<Func<TEntity, object>> property, bool? ascendingOrder = null)
        {
            var existingProperty = PaginationConfiguratorFactory<TEntity>.GetByProperty(str);

            if (existingProperty != null)
            {
                existingProperty.OrderExpressions.Add(new OrderExpression<TEntity>() { PropertyOrderExpression = property, AscendingOrder = ascendingOrder });
                existingProperty.FilterExpression.Add(property);
                return new PaginationExpression<TEntity>(existingProperty);
            }

            var configuration = new PaginationConfiguratorObject<TEntity>();

            configuration.Type = typeof(TEntity);
            configuration.Property = str.ToLower();
            configuration.OrderExpressions.Add(new OrderExpression<TEntity>() { PropertyOrderExpression = property, AscendingOrder = ascendingOrder });
            configuration.FilterExpression.Add(property);

            return new PaginationExpression<TEntity>(PaginationConfiguratorFactory<TEntity>.AddConfiguration(configuration));
        }

        private PaginationExpression<TEntity> CreateGlobalSelect<TEntity>(string str, Expression<Func<TEntity, object>> property)
        {
            var existingProperty = PaginationConfiguratorFactory<TEntity>.GetByProperty(str);

            if (existingProperty != null && existingProperty.SelectExpression != null)
            {
                existingProperty.SelectExpression = property;
                return new PaginationExpression<TEntity>(existingProperty);
            }

            var configuration = new PaginationConfiguratorObject<TEntity>();

            configuration.Type = typeof(TEntity);
            configuration.Property = str.ToLower();
            configuration.SelectExpression = property;

            return new PaginationExpression<TEntity>(PaginationConfiguratorFactory<TEntity>.AddConfiguration(configuration));
        }
    }
}
