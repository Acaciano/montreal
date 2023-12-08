using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;


namespace Montreal.Core.Crosscutting.Infrastructure.Contexts
{
    public class OracleDatabaseConnection : IOracleDatabaseConnection
    {
        private readonly IConfiguration _configuration;

        public OracleDatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public OracleConnection GetConnection()
        {
            var config = new OracleDatabaseConfig();
            _configuration.Bind("Oracle", config);

            if (string.IsNullOrEmpty(config.ConnectionString))
                throw new Exception("Oracle connection is empty.");

            var conn = new OracleConnection(config.ConnectionString);

            return conn;
        }
    }
}