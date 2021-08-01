using System;
using System.Collections.Generic;
using Repository.Models;

namespace Repository
{
    public interface IDataLogService
    {
        public EnvironmentLog Get(long id);

        public EnvironmentLog GetMostRecent();

        public IEnumerable<EnvironmentLog> GetLast(int count);

        public IEnumerable<EnvironmentLog> GetBetweenDates(DateTime start, DateTime end);

        public EnvironmentLog Save(EnvironmentLog environmentLog);
    }
}
