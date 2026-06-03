using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace PurchaseHistory.Infrastructure.Data;

public class DbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(
            _configuration.GetConnectionString("DefaultConnection")
        );
    }
}