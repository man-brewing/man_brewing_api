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
        public DataLog Get(long id)
        {
            using var connection = GetDatabaseConnection();
            return connection.Get<DataLog>(id);
        }

        /// <inheritdoc />
        public DataLog GetMostRecent()
        {
            var query = $"SELECT * FROM {Tables.DataLog} ORDER BY timestamp DESC LIMIT 1;";
            using var connection = GetDatabaseConnection();
            return connection.QuerySingleOrDefault<DataLog>(query);
        }

        /// <inheritdoc />
        public IEnumerable<DataLog> GetLast(int count)
        {
            var query = $"SELECT * FROM {Tables.DataLog} ORDER BY timestamp DESC LIMIT @count;";
            using var connection = GetDatabaseConnection();
            return connection.Query<DataLog>(query, new {count});
        }

        /// <inheritdoc />
        public IEnumerable<DataLog> GetBetweenDates(DateTime start, DateTime end)
        {
            var query =
                $"SELECT * FROM {Tables.DataLog} WHERE timestamp BETWEEN @start AND @end ORDER BY timestamp ASC;";
            using var connection = GetDatabaseConnection();
            return connection.Query<DataLog>(query, new { start, end });
        }

        /// <inheritdoc />
        public DataLog Save(DataLog dataLog)
        {
            using var connection = GetDatabaseConnection();

            if (dataLog.Id > 0)
            {
                connection.Update(dataLog);
            }
            else
            {
                dataLog.Id = connection.Insert(dataLog);
            }

            return dataLog;
        }
    }
}
