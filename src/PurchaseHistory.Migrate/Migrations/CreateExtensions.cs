using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603000)]
public class CreateExtensions : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"pgcrypto\";");
    }

    public override void Down()
    {
        Execute.Sql("DROP EXTENSION IF EXISTS \"pgcrypto\";");
    }
}
