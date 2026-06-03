using Dapper;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public PurchaseRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(
        Purchase purchase)
    {
        const string sql = @"
        INSERT INTO Purchases
        (
            Id,
            UserId,
            StoreId,
            AccessKey,
            PurchaseDate,
            TotalValue,
            XmlFileName,
            CreatedAt
        )
        VALUES
        (
            @Id,
            @UserId,
            @StoreId,
            @AccessKey,
            @PurchaseDate,
            @TotalValue,
            @XmlFileName,
            @CreatedAt
        )";

        purchase.Id = Guid.NewGuid();

        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, purchase);

        return purchase.Id;
    }
}