using Microsoft.Extensions.DependencyInjection;
using Montreal.Core.Crosscutting.Domain.Controller.ActionFilter;

namespace Montreal.Core.Crosscutting.Domain.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddPagedControllers(this IServiceCollection services)
        {
            services.AddControllers(opt => { opt.Filters.Add(new PaginationFilter()); });
        }

        public static void AddAuditLogs(this IServiceCollection services)
        {
            services.AddControllers(opt => { opt.Filters.Add(new AuditFilter()); })
                    .AddNewtonsoftJson(options => 
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }
    }
}
