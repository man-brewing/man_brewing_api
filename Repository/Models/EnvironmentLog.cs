using System;
using Core;
using Dapper.Contrib.Extensions;

namespace Repository.Models
{
    [Table(Tables.EnvironmentLog)]
    public class EnvironmentLog
    {
        [Key]
        public long Id { get; set; }
        public double AmbientTemperatureC { get; set; }
        public double AmbientHumidityPercent { get; set; }
        public double? WeatherTemperatureC { get; set; }
        public double? WeatherHumidityPercent { get; set; }

        [Write(false)]
        public DateTime Timestamp { get; set; }
    }
}
