using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603003)]
public class CreateCouponsImport : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            CREATE TABLE IF NOT EXISTS CouponsImport (
                Id UUID NOT NULL DEFAULT gen_random_uuid(),
                UserId UUID NOT NULL,
                AccessKey VARCHAR(60) NOT NULL,
                Status VARCHAR(20) NOT NULL DEFAULT 'Pending',
                CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
                PRIMARY KEY (Id),
                FOREIGN KEY (UserId) REFERENCES Users(Id)
            );
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            DROP TABLE IF EXISTS CouponsImport;
            """);
    }
}
