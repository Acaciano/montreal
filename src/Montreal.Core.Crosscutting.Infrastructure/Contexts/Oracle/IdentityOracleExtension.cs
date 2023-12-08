using Montreal.Core.Crosscutting.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Montreal.Core.Crosscutting.Infrastructure.Contexts.Oracle
{
    public static class IdentityOracleExtension
    {
        public static void AddIdentityOracleContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration) where TContext : IdentityDbContext
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

            services.AddDefaultIdentity<IdentityUser>()
                   .AddRoles<IdentityRole>()
                   .AddEntityFrameworkStores<TContext>()
                   .AddErrorDescriber<IdentityMensagensPortugues>();

            services.AddScoped<IdentityDbContext, TContext>();
        }
    }
}