using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603005)]
public class AddCategoryIdToPurchaseItems : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            ALTER TABLE PurchaseItems ADD COLUMN IF NOT EXISTS CategoryId UUID;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            ALTER TABLE PurchaseItems DROP COLUMN IF EXISTS CategoryId;
            """);
    }
}
