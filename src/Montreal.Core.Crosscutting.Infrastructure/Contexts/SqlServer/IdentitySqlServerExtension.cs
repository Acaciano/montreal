using Montreal.Core.Crosscutting.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Montreal.Core.Crosscutting.Infrastructure.Contexts.SqlServer
{
    public static class IdentitySqlServerExtension
    {
        public static void AddIdentitySqlServerContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration) where TContext : IdentityDbContext
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

            services.AddDefaultIdentity<IdentityUser>()
                   .AddRoles<IdentityRole>()
                   .AddEntityFrameworkStores<TContext>()
                   .AddErrorDescriber<IdentityMensagensPortugues>();

            services.AddScoped<IdentityDbContext, TContext>();
        }
    }
}