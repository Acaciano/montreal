using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Montreal.Core.Crosscutting.Infrastructure.Contexts.SqlServer
{
    public static class SqlServerExtension
    {
        public static void AddSqlServerContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration) where TContext : DbContext
        {
            var config = new SqlServerContextConfig();
            configuration.Bind("SqlServer", config);

            if (string.IsNullOrEmpty(config.ConnectionString))
                throw new Exception("SqlServer connection is empty.");

            services.AddSingleton(config);

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<TContext>(opt =>
                {
                    opt.UseSqlServer(config.ConnectionString);
                });

            services.AddScoped<DbContext, TContext>();
        }
    }
}