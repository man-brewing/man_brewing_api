using System;
using Core;
using Dapper.Contrib.Extensions;

namespace Repository.Models
{
    [Table(Tables.DataLog)]
    public class DataLog
    {
        [Key]
        public long Id { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime Timestamp { get; set; }
        public double? Ambient_Temp { get; set; }
        public double? Ambient_Humid { get; set; }
    }
}
