using System;
using System.Collections.Generic;
using Repository.Models;

namespace Repository
{
    public interface IDataLogService
    {
        /// <summary>
        /// Gets the log record by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EnvironmentLog Get(long id);

        /// <summary>
        /// Gets the most recently recorded log.
        /// </summary>
        /// <returns></returns>
        public EnvironmentLog GetMostRecent();

        /// <summary>
        /// Gets the most recent <see cref="count"/> records
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<EnvironmentLog> GetLast(int count);

        /// <summary>
        /// Gets logs between the specified dates.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<EnvironmentLog> GetBetweenDates(DateTimeOffset start, DateTimeOffset end);

        /// <summary>
        /// Saves the log record.
        /// </summary>
        /// <param name="environmentLog"></param>
        /// <returns></returns>
        public EnvironmentLog Save(EnvironmentLog environmentLog);
    }
}
