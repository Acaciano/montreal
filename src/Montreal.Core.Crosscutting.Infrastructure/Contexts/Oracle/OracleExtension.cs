using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Montreal.Core.Crosscutting.Infrastructure.Contexts.Oracle
{
    public static class OracleExtension
    {
        public static void AddOracleContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration) where TContext : DbContext
        {
            var config = new OracleDatabaseConfig();
            configuration.Bind("Oracle", config);

            if (string.IsNullOrEmpty(config.ConnectionString))
                throw new Exception("Oracle connection is empty.");

            services.AddSingleton(config);

            services.AddEntityFrameworkOracle()
                .AddDbContext<TContext>(opt =>
                {
                    opt.UseOracle(config.ConnectionString);
                });

            services.AddScoped<DbContext, TContext>();
        }
    }
}