using Dapper;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ProductRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Product?> FindByNameAsync(
        string normalizedName, Guid userId)
    {
        const string sql = @"
        SELECT *
        FROM Products
        WHERE NormalizedName = @NormalizedName
          AND UserId = @UserId
        LIMIT 1";

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Product>(
            sql,
            new
            {
                NormalizedName = normalizedName,
                UserId = userId
            });
    }

    public async Task<Guid> CreateAsync(Product product)
    {
        const string sql = @"
        INSERT INTO Products
        (
            Id,
            UserId,
            NormalizedName,
            Brand,
            CategoryId,
            CreatedAt
        )
        VALUES
        (
            @Id,
            @UserId,
            @NormalizedName,
            @Brand,
            @CategoryId,
            @CreatedAt
        )";

        product.Id = Guid.NewGuid();

        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, product);

        return product.Id;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM Products ORDER BY NormalizedName";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT * FROM Products WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task UpdateAsync(Product product)
    {
        const string sql = @"
            UPDATE Products
            SET NormalizedName = @NormalizedName,
                Brand = @Brand,
                CategoryId = @CategoryId
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, product);
    }

    public async Task UpdateCategoryAsync(Guid id, Guid? categoryId)
    {
        const string sql = @"
            UPDATE Products
            SET CategoryId = @CategoryId
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id, CategoryId = categoryId });
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"DELETE FROM Products WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new { Id = id });
    }
}