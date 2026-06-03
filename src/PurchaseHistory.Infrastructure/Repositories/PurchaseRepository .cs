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

    public async Task<IEnumerable<PurchaseListDto>> GetAllAsync()
    {
        const string sql = @"
            SELECT
                p.Id,
                p.PurchaseDate,
                p.TotalValue,
                s.Name AS StoreName
            FROM Purchases p
            INNER JOIN Stores s ON s.Id = p.StoreId
            ORDER BY p.PurchaseDate DESC";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<PurchaseListDto>(sql);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(
            "DELETE FROM PurchaseItems WHERE PurchaseId = @Id",
            new { Id = id },
            transaction);

        await connection.ExecuteAsync(
            "DELETE FROM Purchases WHERE Id = @Id",
            new { Id = id },
            transaction);

        transaction.Commit();
    }
}