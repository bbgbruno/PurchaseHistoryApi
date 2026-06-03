using Dapper;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;
using static System.Formats.Asn1.AsnWriter;

namespace PurchaseHistory.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public StoreRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Store?> GetByDocumentAsync(
        string document)
    {
        const string sql = @"
        SELECT TOP 1 *
        FROM Stores
        WHERE DocumentNumber = @Document";

        using var connection =
        _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Store>(
            sql,
        new { Document = document });
    }

    public async Task<Guid> CreateAsync(Store store)
    {
        const string sql = @"
        INSERT INTO Stores
        (
            Id,
            Name,
            TradeName,
            DocumentNumber,
            StateRegistration,
            City,
            State,
            CreatedAt
        )
        VALUES
        (
            @Id,
            @Name,
            @TradeName,
            @DocumentNumber,
            @StateRegistration,
            @City,
            @State,
            @CreatedAt
        )";

        store.Id = Guid.NewGuid();

        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, store);

        return store.Id;
    }
}