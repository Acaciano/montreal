using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Montreal.Core.Crosscutting.Common.Data;
using Montreal.Core.Crosscutting.Common.Enum;

namespace Montreal.Core.Crosscutting.Common.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Transform a request QueryParameters to PaginationObject with page, size, filters and ordernations.
        /// </summary>
        /// <remarks>
        /// Example query of a request: "?page=1&size=20&orderProperties=name,true&name=dev"
        /// </remarks>
        /// <param name="request"></param>
        /// <returns>
        /// An <see cref="PaginationObject"/> instanced by request
        /// </returns>
        public static PaginationObject GetPaginationObject(this HttpRequest request, params string[] paramsToIgnore)
        {
            var paginationObject = new PaginationObject();
            var fieldsList = new List<string>();

            foreach (var parameter in request.Query)
            {
                try
                {
                    var property = parameter.Key;
                    var value = parameter.Value.ToString();

                    if (property?.ToLower() == "page")
                    {
                        int.TryParse(value, out var page);
                        paginationObject.Page.Index = page;
                    }
                    else if (property?.ToLower() == "size")
                    {
                        int.TryParse(value, out var size);
                        paginationObject.Page.Quantity = size;
                    }
                    else if (property?.ToLower() == "fields")
                    {
                        foreach (var field in value.Split(',')) fieldsList.Add(field?.Trim());
                    }
                    else if (property?.ToLower() == "orderproperties")
                    {
                        foreach (var orderValue in value.Split(";"))
                        {
                            var orderPropertySplited = orderValue?.Split(",");

                            var orderProperty = orderPropertySplited[0].Trim();
                            var isAscending = false;

                            if (orderPropertySplited.Length > 1)
                            {
                                var ascendingString = orderPropertySplited[1].Trim();
                                isAscending = ascendingString == "ascending";
                            }

                            paginationObject.Ordenations.Add(new Order(orderProperty, isAscending));
                        }
                    }
                    else if (!paramsToIgnore.Any(ignore => ignore.ToLower() == property))
                        paginationObject.Filters.Add(new Filter(property, Condition.Default, value));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return paginationObject;
        }
    }
}
