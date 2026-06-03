using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603004)]
public class AddUserIdToCategories : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            ALTER TABLE Categories ADD COLUMN IF NOT EXISTS UserId UUID;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            ALTER TABLE Categories DROP COLUMN IF EXISTS UserId;
            """);
    }
}
