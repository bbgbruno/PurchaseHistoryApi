using Dapper;
using PurchaseHistory.Domain.Dtos;
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

    public async Task<IEnumerable<PurchaseListDto>> GetAllAsync(Guid userId)
    {
        const string sql = @"
            SELECT
                p.Id,
                p.UserId,
                p.PurchaseDate,
                p.TotalValue,
                s.Name AS StoreName,
                COUNT(pi.Id) AS TotalItems,
                COUNT(CASE WHEN pr.CategoryId IS NOT NULL THEN 1 END) AS CategorizedItems
            FROM Purchases p
            INNER JOIN Stores s ON s.Id = p.StoreId
            LEFT JOIN PurchaseItems pi ON pi.PurchaseId = p.Id
            LEFT JOIN Products pr ON pr.Id = pi.ProductId
            WHERE p.UserId = @UserId
            GROUP BY p.Id, p.UserId, p.PurchaseDate, p.TotalValue, s.Name
            ORDER BY p.PurchaseDate DESC";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<PurchaseListDto>(sql, new { UserId = userId });
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(
            "DELETE FROM PurchaseItems WHERE PurchaseId = @Id",
            new { Id = id },
            transaction);

        await connection.ExecuteAsync(
            "DELETE FROM Purchases WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId },
            transaction);

        transaction.Commit();
    }

    public async Task<bool> ExistsByAccessKeyAsync(string accessKey)
    {
        const string sql = @"
            SELECT COUNT(1) FROM Purchases
            WHERE AccessKey = @AccessKey";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { AccessKey = accessKey });
        return count > 0;
    }

    public async Task<bool> UpdatePurchaseDateAsync(Guid id, DateTime purchaseDate, Guid userId)
    {
        const string sql = @"
            UPDATE Purchases
            SET PurchaseDate = @PurchaseDate
            WHERE Id = @Id AND UserId = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { Id = id, PurchaseDate = purchaseDate, UserId = userId });
        return rows > 0;
    }
}
