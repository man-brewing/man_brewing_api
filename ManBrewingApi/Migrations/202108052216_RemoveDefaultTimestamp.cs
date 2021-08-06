using Core;
using FluentMigrator;
using Repository.Models;

namespace ManBrewingApi.Migrations
{
    [Migration(202108052216)]
    public class RemoveDefaultTimestamp : Migration
    {
        public override void Up()
        {
            Delete.DefaultConstraint().OnTable(Tables.EnvironmentLog).OnColumn(nameof(EnvironmentLog.Timestamp));

            Alter.Table(Tables.EnvironmentLog)
                .AlterColumn(nameof(EnvironmentLog.Timestamp))
                .AsDateTimeOffset().NotNullable();
        }

        public override void Down()
        {
            Alter.Table(Tables.EnvironmentLog)
                .AlterColumn(nameof(EnvironmentLog.Timestamp))
                .AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }
    }
}
