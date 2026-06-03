using Dapper;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Infrastructure.Repositories;

public class ProductKeywordSubstitutionRepository : IProductKeywordSubstitutionRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ProductKeywordSubstitutionRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ProductKeywordSubstitution>> GetAllActiveAsync()
    {
        const string sql = @"
            SELECT * FROM ProductKeywordSubstitutions
            WHERE IsActive = true
            ORDER BY CreatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ProductKeywordSubstitution>(sql);
    }

    public async Task<ProductKeywordSubstitution?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT * FROM ProductKeywordSubstitutions
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ProductKeywordSubstitution>(sql, new { Id = id });
    }

    public async Task CreateAsync(ProductKeywordSubstitution substitution)
    {
        const string sql = @"
            INSERT INTO ProductKeywordSubstitutions
            (Id, OriginalTerm, ReplacementTerm, MatchType, IsActive, CreatedAt, UpdatedAt)
            VALUES
            (@Id, @OriginalTerm, @ReplacementTerm, @MatchType, @IsActive, @CreatedAt, @UpdatedAt)";

        substitution.Id = Guid.NewGuid();
        substitution.CreatedAt = DateTime.UtcNow;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, substitution);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"
            DELETE FROM ProductKeywordSubstitutions
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}
