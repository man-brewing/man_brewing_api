using System.Data;
using System.Data.SqlClient;
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
            _connectionString = configuration["SqlConnectionString"];
        }

        public IDbConnection GetDatabaseConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
