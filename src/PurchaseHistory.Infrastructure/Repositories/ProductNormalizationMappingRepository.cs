using Dapper;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Infrastructure.Repositories;

public class ProductNormalizationMappingRepository : IProductNormalizationMappingRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ProductNormalizationMappingRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ProductNormalizationMapping>> GetAllActiveAsync()
    {
        const string sql = @"
            SELECT * FROM ProductNormalizationMappings
            WHERE IsActive = true
            ORDER BY CreatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ProductNormalizationMapping>(sql);
    }

    public async Task<ProductNormalizationMapping?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT * FROM ProductNormalizationMappings
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ProductNormalizationMapping>(sql, new { Id = id });
    }

    public async Task CreateAsync(ProductNormalizationMapping mapping)
    {
        const string sql = @"
            INSERT INTO ProductNormalizationMappings
            (Id, OriginalText, ReplacementText, MatchType, IsActive, CreatedAt, UpdatedAt)
            VALUES
            (@Id, @OriginalText, @ReplacementText, @MatchType, @IsActive, @CreatedAt, @UpdatedAt)";

        mapping.Id = Guid.NewGuid();
        mapping.CreatedAt = DateTime.UtcNow;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, mapping);
    }

    public async Task UpdateAsync(ProductNormalizationMapping mapping)
    {
        const string sql = @"
            UPDATE ProductNormalizationMappings
            SET OriginalText = @OriginalText,
                ReplacementText = @ReplacementText,
                MatchType = @MatchType,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        mapping.UpdatedAt = DateTime.UtcNow;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, mapping);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"
            DELETE FROM ProductNormalizationMappings
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}
