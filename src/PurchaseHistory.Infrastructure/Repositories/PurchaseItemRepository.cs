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

    public async Task<IEnumerable<ProductSearchResultDto>> SearchProductsAsync(string term)
    {

        const string sql = @"
    SELECT TOP 50

        pi.ProductId,

        pi.Id AS PurchaseItemId,

        pi.OriginalDescription AS ProductName,

        pi.Quantity,

        pi.Unit,

        pi.UnitPrice,

        pi.TotalPrice,

        p.PurchaseDate,

        s.Name AS StoreName

    FROM PurchaseItems pi

    INNER JOIN Purchases p
        ON p.Id = pi.PurchaseId

    INNER JOIN Stores s
        ON s.Id = p.StoreId

    WHERE pi.OriginalDescription
        LIKE @Search

    ORDER BY p.PurchaseDate DESC";

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryAsync<ProductSearchResultDto>(
            sql,
            new
            {
                Search = $"%{term}%"
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

    public async Task<ProductDetailsDto?> GetProductDetailsAsync(Guid productId)
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

    INNER JOIN Purchases p
        ON p.Id = pi.PurchaseId

    INNER JOIN Stores s
        ON s.Id = p.StoreId

    WHERE pi.ProductId = @ProductId

    ORDER BY p.PurchaseDate DESC";

        using var connection =
            _connectionFactory.CreateConnection();

        var items = (
            await connection.QueryAsync<ProductHistoryDto>(
                sql,
                new
                {
                    ProductId = productId
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
}