using Dapper;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM Users ORDER BY Name";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<User>(sql);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT * FROM Users WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = @"SELECT * FROM Users WHERE Email = @Email";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<Guid> CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO Users (Id, Name, Email, PasswordHash, IsActive, CreatedAt)
            VALUES (@Id, @Name, @Email, @PasswordHash, @IsActive, @CreatedAt)";

        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, user);

        return user.Id;
    }

    public async Task UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE Users
            SET Name = @Name,
                Email = @Email,
                IsActive = @IsActive
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, user);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"DELETE FROM Users WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}
