using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603007)]
public class AddUserIdIndexes : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            CREATE INDEX IF NOT EXISTS idx_purchases_user_id ON Purchases(UserId);
            CREATE INDEX IF NOT EXISTS idx_categories_user_id ON Categories(UserId);
            CREATE INDEX IF NOT EXISTS idx_coupons_import_user_id ON CouponsImport(UserId);
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            DROP INDEX IF EXISTS idx_purchases_user_id;
            DROP INDEX IF EXISTS idx_categories_user_id;
            DROP INDEX IF EXISTS idx_coupons_import_user_id;
            """);
    }
}
