using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Data;
using System.IO;
using Montreal.Core.Crosscutting.Infrastructure.Contexts;
using Montreal.Core.Crosscutting.Infrastructure.Contexts.Oracle;
using Montreal.Core.Tests.Contexts;

namespace Montreal.Core.Tests.Services
{
    public class OracleConnectionServiceTest
    {
        public IConfiguration Configuration { get; }
        public IServiceCollection Services;

        public OracleConnectionServiceTest()
        {
            string APP_SETTINGS = "appsettings.json";

            Configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile(APP_SETTINGS, optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DBContextTest()
        {
            Services = new ServiceCollection();

            Services.AddOracleContext<IdentityContext>(Configuration);

            var provider = Services.BuildServiceProvider();

            Assert.That(provider, Is.Not.Null);
        }

        [Test]
        public void ConnTest()
        {
            var oracle = new OracleDatabaseConnection(Configuration);
            var conn = oracle.GetConnection();

            conn.Open();

            Assert.IsTrue(conn.State == ConnectionState.Open);
        }
    }
}