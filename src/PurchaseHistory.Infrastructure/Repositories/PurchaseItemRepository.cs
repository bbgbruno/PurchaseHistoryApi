using Dapper;
using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Infrastructure.Repositories;

public class PurchaseItemRepository
    : IPurchaseItemRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public PurchaseItemRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SaveAsync(
        IEnumerable<PurchaseItem> items)
    {
        const string sql = @"
        INSERT INTO PurchaseItems
        (
            Id,
            PurchaseId,
            ProductId,
            OriginalDescription,
            ProductCode,
            NcmCode,
            Ean,
            Quantity,
            Unit,
            UnitPrice,
            TotalPrice,
            CreatedAt
        )
        VALUES
        (
            @Id,
            @PurchaseId,
            @ProductId,
            @OriginalDescription,
            @ProductCode,
            @NcmCode,
            @Ean,
            @Quantity,
            @Unit,
            @UnitPrice,
            @TotalPrice,
            @CreatedAt
        )";

        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, items);
    }

    public async Task<IEnumerable<ProductSearchResultDto>> SearchProductsAsync(string term, Guid userId)
    {
        const string sql = @"
            SELECT
                pi.ProductId,
                pi.Id AS PurchaseItemId,
                pr.NormalizedName AS ProductName,
                pi.Quantity,
                pi.Unit,
                pi.UnitPrice,
                pi.TotalPrice,
                p.PurchaseDate,
                s.Name AS StoreName
            FROM PurchaseItems pi
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            INNER JOIN Stores s ON s.Id = p.StoreId
            INNER JOIN Products pr ON pr.Id = pi.ProductId
            WHERE pr.NormalizedName ILIKE @Search
              AND p.UserId = @UserId
            ORDER BY p.PurchaseDate DESC
            LIMIT 50";

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryAsync<ProductSearchResultDto>(
            sql,
            new
            {
                Search = $"%{term}%",
                UserId = userId
            });
    }

    public async Task RedirectProductIdAsync(Guid oldProductId, Guid newProductId)
    {
        const string sql = @"
            UPDATE PurchaseItems
            SET ProductId = @NewProductId
            WHERE ProductId = @OldProductId";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            OldProductId = oldProductId,
            NewProductId = newProductId
        });
    }

    public async Task<IEnumerable<PurchaseItem>> GetByProductIdAsync(Guid productId)
    {
        const string sql = @"SELECT * FROM PurchaseItems WHERE ProductId = @ProductId";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<PurchaseItem>(sql, new { ProductId = productId });
    }

    public async Task<ProductDetailsDto?> GetProductDetailsAsync(Guid productId, Guid userId)
    {
        const string sql = @"
            SELECT
                pi.ProductId,
                pi.OriginalDescription AS ProductName,
                pi.UnitPrice,
                pi.TotalPrice,
                pi.Quantity,
                s.Name AS StoreName,
                p.PurchaseDate
            FROM PurchaseItems pi
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            INNER JOIN Stores s ON s.Id = p.StoreId
            WHERE pi.ProductId = @ProductId
              AND p.UserId = @UserId
            ORDER BY p.PurchaseDate DESC";

        using var connection =
            _connectionFactory.CreateConnection();

        var items = (
            await connection.QueryAsync<ProductHistoryDto>(
                sql,
                new
                {
                    ProductId = productId,
                    UserId = userId
                })
        ).ToList();

        if (!items.Any())
            return null;

        return new ProductDetailsDto
        {
            ProductId = productId,

            ProductName =
                items.First().ProductName,

            LowestPrice =
                items.Min(x => x.UnitPrice),

            HighestPrice =
                items.Max(x => x.UnitPrice),

            AveragePrice =
                Math.Round(
                    items.Average(x => x.UnitPrice),
                    2),

            History = items
        };
    }

    public async Task<IEnumerable<PurchaseItem>> GetByPurchaseIdAsync(Guid purchaseId, Guid userId)
    {
        const string sql = @"
            SELECT
                pi.*,
                pr.CategoryId
            FROM PurchaseItems pi
            LEFT JOIN Products pr ON pr.Id = pi.ProductId
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            WHERE pi.PurchaseId = @PurchaseId
              AND p.UserId = @UserId
            ORDER BY pi.OriginalDescription";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<PurchaseItem>(sql, new { PurchaseId = purchaseId, UserId = userId });
    }

    public async Task<PurchaseItem?> GetByIdAsync(Guid id, Guid userId)
    {
        const string sql = @"
            SELECT
                pi.*,
                pr.CategoryId
            FROM PurchaseItems pi
            LEFT JOIN Products pr ON pr.Id = pi.ProductId
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            WHERE pi.Id = @Id
              AND p.UserId = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<PurchaseItem>(sql, new { Id = id, UserId = userId });
    }
}
