using Dapper;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public CategoryRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(Guid userId)
    {
        const string sql = @"
            SELECT * FROM Categories
            WHERE UserId = @UserId
            ORDER BY Name";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Category>(sql, new { UserId = userId });
    }

    public async Task<Category?> GetByIdAsync(Guid id, Guid userId)
    {
        const string sql = @"
            SELECT * FROM Categories
            WHERE Id = @Id AND UserId = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id, UserId = userId });
    }

    public async Task<Guid> CreateAsync(Category category)
    {
        const string sql = @"
            INSERT INTO Categories (Id, Name, UserId, CreatedAt)
            VALUES (@Id, @Name, @UserId, @CreatedAt)";

        category.Id = Guid.NewGuid();
        category.CreatedAt = DateTime.UtcNow;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, category);

        return category.Id;
    }

    public async Task UpdateAsync(Category category)
    {
        const string sql = @"
            UPDATE Categories
            SET Name = @Name
            WHERE Id = @Id AND UserId = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, category);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        const string sql = @"DELETE FROM Categories WHERE Id = @Id AND UserId = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
    }
}
