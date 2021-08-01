using Core;
using FluentMigrator;
using Repository.Models;

namespace ManBrewingApi.Migrations
{
    [Migration(202108011516)]
    public class CreateEnvironmentTable : Migration
    {
        public override void Up()
        {
            Create.Table(Tables.EnvironmentLog)
                .WithColumn(nameof(EnvironmentLog.Id)).AsInt64().NotNullable().Identity().PrimaryKey()
                .WithColumn(nameof(EnvironmentLog.AmbientTemperatureC)).AsDouble().NotNullable()
                .WithColumn(nameof(EnvironmentLog.AmbientHumidityPercent)).AsDouble().NotNullable()
                .WithColumn(nameof(EnvironmentLog.WeatherTemperatureC)).AsDouble().Nullable()
                .WithColumn(nameof(EnvironmentLog.WeatherHumidityPercent)).AsDouble().Nullable()
                .WithColumn(nameof(EnvironmentLog.Timestamp)).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            Delete.Table(Tables.EnvironmentLog);
        }
    }
}
