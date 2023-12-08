using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Common.Data;

namespace Montreal.Core.Crosscutting.Common.Extensions.V1
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Asynchronous create a <see cref="PagedList{TEntity}"/> from an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="pagination"><see cref="Page"/> object to get the page and the size for <see cref="PagedList{TEntity}"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PagedList{TEntity}"/> that contains elements from the input sequence. </returns>
        /// <exception cref="ArgumentNullException"><see cref="Page"/> object cannot be null.</exception>
        public static async Task<PagedList<TType>> ToPagedListAsync<TType>(this IQueryable<TType> cursor, Page pagination) where TType : class
        {
            pagination.Index = pagination.Index <= 0 ? pagination.Index = 1 : pagination.Index;
            pagination.Quantity = pagination.Quantity <= 0 ? pagination.Quantity = 20 : pagination.Quantity;

            var pagedList = new PagedList<TType>();

            pagedList.TotalRecords = cursor.AsNoTracking().Count();
            pagedList.TotalPages = (int)Math.Ceiling(pagedList.TotalRecords / (double)pagination.Quantity);
            pagedList.CurrentPage = pagination.Index;
            pagedList.PageSize = pagination.Quantity;
            pagedList.Results = await cursor.Skip((pagination.Index - 1) * pagination.Quantity).Take(pagination.Quantity).ToListAsync();

            return pagedList;
        }
    }
}