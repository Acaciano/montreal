using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using Montreal.Core.Crosscutting.Common.Extensions;

namespace Montreal.Core.Crosscutting.Domain.Controller.ActionFilter
{
    internal class PaginationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var paramsToIgnore = context?.ActionArguments?.Keys?.ToArray();
            (context.Controller as ApiController).PaginationObject = context.HttpContext.Request.GetPaginationObject(paramsToIgnore);
        }
    }
}
