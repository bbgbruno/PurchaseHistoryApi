using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603009)]
public class BackfillProductUserIds : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            UPDATE Products p
            SET UserId = sub.UserId
            FROM (
                SELECT DISTINCT ON (pi.ProductId) pi.ProductId, pu.UserId
                FROM PurchaseItems pi
                INNER JOIN Purchases pu ON pu.Id = pi.PurchaseId
                WHERE pi.ProductId IS NOT NULL
                ORDER BY pi.ProductId, pu.PurchaseDate DESC NULLS LAST
            ) sub
            WHERE p.Id = sub.ProductId
              AND p.UserId IS NULL;
            """);
    }

    public override void Down()
    {
    }
}
