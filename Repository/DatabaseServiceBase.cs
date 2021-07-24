using System.Data;
using Core;
using Core.OptionBinders;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Repository
{
    public class DatabaseServiceBase
    {
        private readonly string _connectionString;

        public DatabaseServiceBase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(nameof(ConnectionStrings.MySql));
        }

        public IDbConnection GetDatabaseConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
