using Dapper;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Infrastructure.Repositories;

public class CouponImportRepository : ICouponImportRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public CouponImportRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(CouponImport couponImport)
    {
        const string sql = @"
            INSERT INTO CouponsImport (Id, UserId, AccessKey, Status, CreatedAt)
            VALUES (@Id, @UserId, @AccessKey, @Status, @CreatedAt)";

        couponImport.Id = Guid.NewGuid();
        couponImport.CreatedAt = DateTime.UtcNow;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, couponImport);

        return couponImport.Id;
    }

    public async Task<IEnumerable<CouponImport>> GetPendingAsync(Guid userId)
    {
        const string sql = @"
            SELECT * FROM CouponsImport
            WHERE Status = 'Pending'
              AND UserId = @UserId
            ORDER BY CreatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<CouponImport>(sql, new { UserId = userId });
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"DELETE FROM CouponsImport WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task UpdateStatusAsync(Guid id, string status)
    {
        const string sql = @"UPDATE CouponsImport SET Status = @Status WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id, Status = status });
    }
}
