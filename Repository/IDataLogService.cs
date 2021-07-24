using System;
using System.Collections.Generic;
using Repository.Models;

namespace Repository
{
    public interface IDataLogService
    {
        public DataLog Get(long id);

        public DataLog GetMostRecent();

        public IEnumerable<DataLog> GetLast(int count);

        public IEnumerable<DataLog> GetBetweenDates(DateTime start, DateTime end);

        public DataLog Save(DataLog dataLog);
    }
}
