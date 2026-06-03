using BCrypt.Net;
using FluentMigrator;

namespace PurchaseHistory.Migrate.Migrations;

[Migration(20260603002)]
public class SeedAdminUser : Migration
{
    public override void Up()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");

        Execute.Sql($$"""
            INSERT INTO Users (Id, Name, Email, PasswordHash, IsActive, CreatedAt)
            VALUES (gen_random_uuid(), 'Bruno Borges', 'admin@purchasehistory.local', '{{passwordHash}}', TRUE, NOW())
            ON CONFLICT (Email) DO NOTHING;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            DELETE FROM Users WHERE Email = 'admin@purchasehistory.local';
            """);
    }
}
