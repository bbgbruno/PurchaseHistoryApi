using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603006)]
public class MoveCategoryIdToProducts : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            ALTER TABLE PurchaseItems DROP COLUMN IF EXISTS CategoryId;
            ALTER TABLE Products ADD COLUMN IF NOT EXISTS CategoryId UUID;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            ALTER TABLE Products DROP COLUMN IF EXISTS CategoryId;
            ALTER TABLE PurchaseItems ADD COLUMN IF NOT EXISTS CategoryId UUID;
            """);
    }
}
