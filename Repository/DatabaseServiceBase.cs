using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Repository
{
    public class DatabaseServiceBase
    {
        private readonly string _connectionString;

        public DatabaseServiceBase(IConfiguration configuration)
        {
            _connectionString = configuration["SqlConnectionString"];
        }

        /// <summary>
        /// Gets a database connection from the pool.
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetDatabaseConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
