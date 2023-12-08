using Oracle.ManagedDataAccess.Client;

namespace Montreal.Core.Crosscutting.Infrastructure.Contexts
{
    public interface IOracleDatabaseConnection
    {
        OracleConnection GetConnection();
    }
}