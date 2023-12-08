using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Montreal.Core.Crosscutting.Common.Data;
using Montreal.Core.Crosscutting.Common.Enum;
using Montreal.Core.Crosscutting.Common.Exceptions;
using Montreal.Core.Crosscutting.Common.Pagination;

namespace Montreal.Core.Crosscutting.Common.Extensions
{
    public static class QueryableExtensions
    {
        #region Private Methods

        private static IPaginationQueryable<TEntity> FilterByFilters<TEntity>(this IQueryable<TEntity> originalQuery, ICollection<Filter> filters) where TEntity : class
        {
            if (filters == null) return new PaginationQueryable<TEntity>(originalQuery);

            var query = (originalQuery as PaginationQueryable<TEntity>);
            var expressionParams = Expression.Parameter(typeof(TEntity));
            var lastWhereExpression = (Expression)null;

            foreach (var filter in filters)
            {
                try
                {
                    var customConfig = query?.Configurations?.FirstOrDefault(x => x.Property.ToLower() == filter.Property.ToLower()
                                                                                       && x.FilterExpression != null && x.FilterExpression.Any());
                    var customFiltersExpression = customConfig?.FilterExpression;
                    var hasGlobalConfig = query?.HasGlobalConfig ?? false;
                    var hasAllFilter = query?.HasAllFilter ?? false;

                    if ((customFiltersExpression == null || !customFiltersExpression.Any()) && hasGlobalConfig)
                        customFiltersExpression = PaginationConfiguratorFactory<TEntity>.GetOrderConfiguration(filter.Property)?.FilterExpression;

                    if ((customFiltersExpression == null || !customFiltersExpression.Any()) && hasAllFilter)
                    {
                        var whereExpression = CreateFilterExpression<TEntity>(expressionParams, filter);

                        lastWhereExpression = lastWhereExpression != null ? Expression.And(lastWhereExpression, whereExpression)
                                                                      : whereExpression;
                        continue;
                    }

                    if (customFiltersExpression == null || !customFiltersExpression.Any())
                        throw new FilterNotConfiguredException();

                    foreach (var customFilter in customFiltersExpression)
                    {
                        var filterBody = customFilter?.StripConvert()?.Body;

                        var whereExpression = CreateFilterExpression<TEntity>(expressionParams, filter, filterBody);

                        lastWhereExpression = lastWhereExpression != null ? Expression.And(lastWhereExpression, whereExpression)
                                                                      : whereExpression;
                    }
                }
                catch (PaginationException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    continue;
                }
            }

            if (lastWhereExpression != null)
            {
                var filterLambda = Expression.Lambda<Func<TEntity, bool>>(lastWhereExpression, expressionParams);
                return new PaginationQueryable<TEntity>(query.Where(filterLambda), query.Configurations, query.HasAllOrder, query.HasAllFilter, query.HasGlobalConfig);
            }

            return new PaginationQueryable<TEntity>(query, query.Configurations, query.HasAllOrder, query.HasAllFilter, query.HasGlobalConfig);
        }

        private static Expression CreateFilterExpression<TEntity>(ParameterExpression expressionParams, Filter filter, Expression customFilter = null)
        {
            var memberExpression = customFilter != null ? customFilter.ReplaceParameter(expressionParams)
                                                        : filter.Property.ToMemberExpression<TEntity>(expressionParams);

            var isEnum = memberExpression?.Type?.IsEnum ?? false;
            var isDateTime = memberExpression?.Type?.IsDateTime() ?? false;
            var isDecimal = memberExpression?.Type?.IsDecimal() ?? false;

            if (isDateTime)
            {
                memberExpression = customFilter == null ? Expression.Property(memberExpression, "date") : memberExpression;
                filter.Value = filter.Value.ConvertToCustomCultureDateString(CultureInfo.CreateSpecificCulture("pt-br"));
            }
            else if (isEnum)
            {
                memberExpression = Expression.Convert(memberExpression, typeof(int));
            }
            else if (isDecimal)
            {
                var notNullableType = Nullable.GetUnderlyingType(memberExpression.Type);

                var convertMethod = typeof(Convert).GetMethods().FirstOrDefault(x => x.IsPublic &&
                                                                                     x.IsStatic &&
                                                                                     x.GetParameters().All(x => x.ParameterType == (notNullableType ?? memberExpression.Type)) &&
                                                                                     x.Name == "ToDecimal");
                if (convertMethod != null)
                {
                    var isNullable = notNullableType != null;

                    if (isNullable) memberExpression = Expression.Property(memberExpression, "Value");

                    memberExpression = Expression.Call(convertMethod, memberExpression);
                }
            }

            if (memberExpression?.Type != typeof(string))
                memberExpression = Expression.Call(memberExpression, "ToString", null);

            var propertyToLower = Expression.Call(memberExpression, "ToLower", null);

            var value = Expression.Constant(filter.Value.ToLower(), typeof(string));
            return (Expression)propertyToLower.GetConditionExpression(value, filter.Condition);
        }

        private static IPaginationQueryable<TEntity> OrderMultiple<TEntity>(this IQueryable<TEntity> originalQuery, params Order[] ordernations) where TEntity : class
        {
            if (ordernations == null) return (originalQuery as PaginationQueryable<TEntity>);

            var query = (originalQuery as PaginationQueryable<TEntity>);
            var lastQuery = (IOrderedQueryable<TEntity>)null;

            foreach (var order in ordernations)
            {
                try
                {
                    var customConfig = query?.Configurations?.FirstOrDefault(x => x.Property.ToLower() == order.Property.ToLower()
                                                                                       && x.OrderExpressions != null && x.OrderExpressions.Any());

                    var customOrderConfigurations = customConfig?.OrderExpressions;
                    var hasGlobalConfig = query?.HasGlobalConfig ?? false;
                    var hasAllOrder = query?.HasAllOrder ?? false;

                    if ((customOrderConfigurations == null || !customOrderConfigurations.Any()) && hasGlobalConfig)
                        customOrderConfigurations = PaginationConfiguratorFactory<TEntity>.GetOrderConfiguration(order.Property)?.OrderExpressions;

                    if ((customOrderConfigurations == null || !customOrderConfigurations.Any()) && hasAllOrder)
                    {
                        var orderExpression = CreateOrderExpression<TEntity>(order);

                        lastQuery = lastQuery == null ? query.Order(orderExpression, order.Crescent)
                                                      : lastQuery.ThenOrder(orderExpression, order.Crescent);
                        continue;
                    }

                    if (customOrderConfigurations == null || !customOrderConfigurations.Any())
                        throw new OrdenationNotConfiguredException();

                    foreach (var customOrder in customOrderConfigurations)
                    {
                        var expressionBody = customOrder?.PropertyOrderExpression?.StripConvert()?.Body ?? customOrder?.BoolOrderExpression?.Body;

                        var orderExpression = CreateOrderExpression<TEntity>(order, expressionBody);

                        lastQuery = lastQuery == null ? query.Order(orderExpression, customOrder.AscendingOrder ?? order.Crescent)
                                                      : lastQuery.ThenOrder(orderExpression, customOrder.AscendingOrder ?? order.Crescent);
                    }
                }
                catch (PaginationException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    continue;
                }
            }

            return lastQuery != null ? new PaginationQueryable<TEntity>(lastQuery, query.Configurations, query.HasAllOrder, query.HasAllFilter, query.HasGlobalConfig)
                                     : query;
        }

        private static Expression<Func<TEntity, object>> CreateOrderExpression<TEntity>(Order order, Expression customOrder = null)
        {
            var expressionParams = Expression.Parameter(typeof(TEntity));
            var memberExpression = customOrder != null ? customOrder.ReplaceParameter(expressionParams)
                                                       : order.Property.ToMemberExpression<TEntity>(expressionParams);

            var castExpressionToObject = Expression.Convert(memberExpression, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(castExpressionToObject, expressionParams);
        }

        private static bool IsDateTime(this Type type) => type == typeof(DateTime) || type == typeof(DateTime?);

        private static bool IsDecimal(this Type type) =>
               type == typeof(double) || type == typeof(double?)
            || type == typeof(float) || type == typeof(float?)
            || type == typeof(decimal) || type == typeof(decimal?);

        private static Expression GetConditionExpression(this Expression propertyExpression, Expression value, Condition conditionType) => conditionType switch
        {
            Condition.Equal => Expression.Equal(propertyExpression, value),
            Condition.Different => Expression.Not(Expression.Equal(propertyExpression, value)),
            Condition.StartsWith => Expression.Call(propertyExpression, "StartsWith", null, value),
            Condition.EndsWith => Expression.Call(propertyExpression, "EndsWith", null, value),
            _ => Expression.Call(propertyExpression, "Contains", null, value)
        };

        private static IQueryable<TEntity> IncludeByString<TEntity>(this IQueryable<TEntity> query, params string[] strings) where TEntity : class
        {
            foreach (var str in strings)
            {
                var propertyExpression = str?.GetPropertyToInclude<TEntity, object>();
                query = propertyExpression != null ? query.Include(propertyExpression) : query;
            }

            return query;
        }

        #endregion

        /// <summary>
        /// Specifies related entities to include in the query results. The navigation property
        /// to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <returns>A new query with the related data included.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<TEntity> IncludeProperties<TEntity>(this IQueryable<TEntity> entities,
            params Expression<Func<TEntity, object>>[] properties) where TEntity : class
        {
            if (properties != null)
                entities = properties.Aggregate(entities, (current, include) => current.Include(include));

            return entities;
        }

        /// <summary>
        /// Specifies related entities to include in the query results from an <see cref="PaginationObject"/>. The navigation property
        /// to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <returns>A new query with the related data included.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<TEntity> IncludeByPaginationObject<TEntity>(this IQueryable<TEntity> query, PaginationObject pagination, params Expression<Func<TEntity, object>>[] includeExpressions) where TEntity : class
        {
            try
            {
                foreach (var includeExpression in includeExpressions)
                    query = query.Include(includeExpression);

                var allPropertiesToInclude = new List<string>();

                allPropertiesToInclude.AddRange(pagination?.Ordenations?.Select(x => x?.Property));
                allPropertiesToInclude.AddRange(pagination?.Filters?.Select(x => x?.Property));

                if (!allPropertiesToInclude.Any()) return (IOrderedQueryable<TEntity>)query;

                return query.IncludeByString(allPropertiesToInclude.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return query;
            }
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to a key.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IOrderedQueryable<TEntity> Order<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, object>> propertyExpression, bool ascendingOrder = true)
            => ascendingOrder ? query.OrderBy(propertyExpression) : query.OrderByDescending(propertyExpression);

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending or descending order
        /// according to a key.
        /// </summary>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to a key.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IOrderedQueryable<TEntity> ThenOrder<TEntity>(this IOrderedQueryable<TEntity> query, Expression<Func<TEntity, object>> propertyExpression, bool ascendingOrder = true)
            => ascendingOrder ? query.ThenBy(propertyExpression) : query.ThenByDescending(propertyExpression);

        /// <summary>
        /// Generates a custom configuration for an literalName thats can represent a property to order and filter.
        /// </summary>
        /// <remarks>To generate a order or filter configuration, call <see cref="PaginationConfigQueryable{TEntity}.AddOrder(Expression{Func{TEntity, bool}}, bool?)"/>
        /// or <see cref="PaginationConfigQueryable{TEntity}.AddFilter(Expression{Func{TEntity, object}})"/>.</remarks>
        /// <param name="pseudoName">Names to represent a property later.</param>
        /// <returns>An <see cref="IPaginationConfigQueryable{TEntity}"/> containing the custom configurations.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IPaginationConfigQueryable<TEntity> CreateCustomConfigFor<TEntity>(this IQueryable<TEntity> query, string literalName)
        {
            var config = new PaginationConfiguratorObject<TEntity>();
            config.Type = typeof(TEntity);
            config.Property = literalName;

            return new PaginationConfigQueryable<TEntity>(config, query);
        }

        /// <summary>
        /// Apply the <see cref="Common.Data.Order"/> based on previously configurations. In case of not configuration, throws a new exception.
        /// </summary>
        /// <remarks>To configure previously, call <see cref="CreateCustomConfigFor{TEntity}(IQueryable{TEntity}, string)"/> 
        /// or <see cref="AllowOrderByAllColumns{TEntity}(IQueryable{TEntity}, bool)"/></remarks>
        /// <returns>An <see cref="IPaginationQueryable{TEntity}"/> with the sorts methods applied in the original query.</returns>
        /// <exception cref="OrdenationNotConfiguredException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<TEntity> ApplyCustomOrders<TEntity>(this IQueryable<TEntity> query, params Order[] orders) where TEntity : class
            => query.OrderMultiple(orders);

        /// <summary>
        /// Apply the <see cref="Filter"/> based on previously configurations. In case of not configuration, throws a new exception.
        /// </summary>
        /// <remarks>To configure previously, call <see cref="CreateCustomConfigFor{TEntity}(IQueryable{TEntity}, string)"/> 
        /// or <see cref="AllowFilterByAllColumns{TEntity}(IQueryable{TEntity}, bool){TEntity}(IQueryable{TEntity}, bool)"/></remarks>
        /// <returns>An <see cref="IPaginationQueryable{TEntity}"/> with the where methods applied in the original query.</returns>
        /// <exception cref="FilterNotConfiguredException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<TEntity> ApplyCustomFilters<TEntity>(this IQueryable<TEntity> query, params Filter[] filters) where TEntity : class
            => query.FilterByFilters(filters);

        /// <summary>
        /// Enable or Disable the <see cref="ApplyCustomOrders{TEntity}(IQueryable{TEntity}, Order[])"/> order by all collumns using navigation property.
        /// </summary>
        /// <remarks>The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// Further navigation properties to be included can be appended, separated by the '.' character.</remarks>
        /// <returns>A <see cref="IPaginationQueryable{TEntity}"/> containing the permission.</returns>
        public static IQueryable<TEntity> AllowOrderByAllColumns<TEntity>(this IQueryable<TEntity> query, bool allow = true) where TEntity : class
            => new PaginationQueryable<TEntity>(query, hasAllorder: allow);

        /// <summary>
        /// Enable or Disable the <see cref="ApplyCustomOrders{TEntity}(IQueryable{TEntity}, Order[])"/> filter by all collumns using navigation property.
        /// </summary>
        /// <remarks>The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// Further navigation properties to be included can be appended, separated by the '.' character.</remarks>
        /// <returns>A <see cref="IPaginationQueryable{TEntity}"/> containing the permission.</returns>
        public static IQueryable<TEntity> AllowFilterByAllColumns<TEntity>(this IQueryable<TEntity> query, bool allow = true) where TEntity : class
            => new PaginationQueryable<TEntity>(query, hasAllFilter: allow);

        /// <summary>
        /// Enable or Disable the <see cref="ApplyCustomOrders{TEntity}(IQueryable{TEntity}, Order[])"/> and <see cref="ApplyCustomFilters{TEntity}(IQueryable{TEntity}, Filter[])"/>
        /// to get global configurations, instanced by <see cref="PaginationCustomConfiguration"/>.
        /// </summary>
        /// <returns>A <see cref="IPaginationQueryable{TEntity}"/> containing the permission.</returns>
        public static IQueryable<TEntity> AllowGlobalPaginationConfig<TEntity>(this IQueryable<TEntity> query, bool allow = true) where TEntity : class
            => new PaginationQueryable<TEntity>(query, hasGlobalConfig: allow);
    }
}