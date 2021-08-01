using System;
using System.Collections.Generic;
using Core;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Repository.Models;

namespace Repository
{
    public class DataLogService : DatabaseServiceBase, IDataLogService
    {
        public DataLogService(IConfiguration configuration) : base(configuration)
        {
        }

        /// <inheritdoc />
        public EnvironmentLog Get(long id)
        {
            using var connection = GetDatabaseConnection();
            return connection.Get<EnvironmentLog>(id);
        }

        /// <inheritdoc />
        public EnvironmentLog GetMostRecent()
        {
            var query = $"SELECT TOP 1 * FROM {Tables.EnvironmentLog} ORDER BY timestamp DESC";
            using var connection = GetDatabaseConnection();
            return connection.QuerySingleOrDefault<EnvironmentLog>(query);
        }

        /// <inheritdoc />
        public IEnumerable<EnvironmentLog> GetLast(int count)
        {
            var query = $"SELECT TOP @count * FROM {Tables.EnvironmentLog} ORDER BY timestamp DESC";
            using var connection = GetDatabaseConnection();
            return connection.Query<EnvironmentLog>(query, new {count});
        }

        /// <inheritdoc />
        public IEnumerable<EnvironmentLog> GetBetweenDates(DateTime start, DateTime end)
        {
            var query =
                $"SELECT * FROM {Tables.EnvironmentLog} WHERE timestamp BETWEEN @start AND @end ORDER BY timestamp ASC";
            using var connection = GetDatabaseConnection();
            return connection.Query<EnvironmentLog>(query, new { start, end });
        }

        /// <inheritdoc />
        public EnvironmentLog Save(EnvironmentLog environmentLog)
        {
            using var connection = GetDatabaseConnection();

            if (environmentLog.Id > 0)
            {
                connection.Update(environmentLog);
            }
            else
            {
                environmentLog.Id = connection.Insert(environmentLog);
            }

            return environmentLog;
        }
    }
}
