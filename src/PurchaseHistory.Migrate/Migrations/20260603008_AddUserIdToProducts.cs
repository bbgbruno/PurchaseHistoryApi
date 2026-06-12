using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603008)]
public class AddUserIdToProducts : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            ALTER TABLE Products ADD COLUMN IF NOT EXISTS UserId UUID;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            ALTER TABLE Products DROP COLUMN IF EXISTS UserId;
            """);
    }
}
