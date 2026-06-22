using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603010)]
public class AddDiscountToPurchaseItems : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            ALTER TABLE PurchaseItems ADD COLUMN IF NOT EXISTS Discount NUMERIC(18,4) NOT NULL DEFAULT 0;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            ALTER TABLE PurchaseItems DROP COLUMN IF EXISTS Discount;
            """);
    }
}
