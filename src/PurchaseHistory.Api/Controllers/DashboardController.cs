using Dapper;
using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    [HttpGet("category/{categoryId}/products")]
    public async Task<IActionResult> GetCategoryProducts(
        Guid categoryId,
        [FromQuery] Guid userId,
        [FromServices] DbConnectionFactory connectionFactory)
    {
        using var connection = connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                pr.Id AS ProductId,
                pr.NormalizedName AS ProductName,
                SUM(pi.Quantity) AS Quantity,
                SUM(pi.TotalPrice) AS TotalPrice
            FROM PurchaseItems pi
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            INNER JOIN Products pr ON pr.Id = pi.ProductId
            WHERE pr.CategoryId = @CategoryId
              AND p.UserId = @UserId
              AND p.PurchaseDate >= DATE_TRUNC('month', NOW() - INTERVAL '1 month')
              AND p.PurchaseDate < DATE_TRUNC('month', NOW() + INTERVAL '1 month')
            GROUP BY pr.Id, pr.NormalizedName";

        var all = (await connection.QueryAsync<CategoryProductItemDto>(sql, new { CategoryId = categoryId, UserId = userId })).ToList();

        var currentMonthSql = @"
            SELECT
                pr.Id AS ProductId,
                pr.NormalizedName AS ProductName,
                SUM(pi.Quantity) AS Quantity,
                SUM(pi.TotalPrice) AS TotalPrice
            FROM PurchaseItems pi
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            INNER JOIN Products pr ON pr.Id = pi.ProductId
            WHERE pr.CategoryId = @CategoryId
              AND p.UserId = @UserId
              AND p.PurchaseDate >= DATE_TRUNC('month', NOW())
              AND p.PurchaseDate < DATE_TRUNC('month', NOW() + INTERVAL '1 month')
            GROUP BY pr.Id, pr.NormalizedName";

        var currentMonth = (await connection.QueryAsync<CategoryProductItemDto>(currentMonthSql, new { CategoryId = categoryId, UserId = userId })).ToList();

        var lastMonthSql = @"
            SELECT
                pr.Id AS ProductId,
                pr.NormalizedName AS ProductName,
                SUM(pi.Quantity) AS Quantity,
                SUM(pi.TotalPrice) AS TotalPrice
            FROM PurchaseItems pi
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            INNER JOIN Products pr ON pr.Id = pi.ProductId
            WHERE pr.CategoryId = @CategoryId
              AND p.UserId = @UserId
              AND p.PurchaseDate >= DATE_TRUNC('month', NOW() - INTERVAL '1 month')
              AND p.PurchaseDate < DATE_TRUNC('month', NOW())
            GROUP BY pr.Id, pr.NormalizedName";

        var lastMonth = (await connection.QueryAsync<CategoryProductItemDto>(lastMonthSql, new { CategoryId = categoryId, UserId = userId })).ToList();

        var categoryName = await connection.ExecuteScalarAsync<string>(
            "SELECT Name FROM Categories WHERE Id = @Id", new { Id = categoryId });

        return Ok(new CategoryProductsDto
        {
            CategoryId = categoryId,
            CategoryName = categoryName ?? "",
            CurrentMonth = currentMonth,
            LastMonth = lastMonth
        });
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid userId,
        [FromServices] DbConnectionFactory connectionFactory)
    {
        using var connection = connectionFactory.CreateConnection();

        var currentMonthSql = @"
            SELECT COALESCE(SUM(TotalValue), 0)
            FROM Purchases
            WHERE UserId = @UserId
              AND PurchaseDate >= DATE_TRUNC('month', NOW())";

        var currentTotal = await connection.ExecuteScalarAsync<decimal>(currentMonthSql, new { UserId = userId });

        var lastMonthSql = @"
            SELECT COALESCE(SUM(TotalValue), 0)
            FROM Purchases
            WHERE UserId = @UserId
              AND PurchaseDate >= DATE_TRUNC('month', NOW() - INTERVAL '1 month')
              AND PurchaseDate < DATE_TRUNC('month', NOW())";

        var lastTotal = await connection.ExecuteScalarAsync<decimal>(lastMonthSql, new { UserId = userId });

        var categoriesSql = @"
            SELECT
                c.Id AS CategoryId,
                c.Name AS CategoryName,
                COALESCE(SUM(CASE
                    WHEN p.PurchaseDate >= DATE_TRUNC('month', NOW())
                    THEN pi.TotalPrice ELSE 0
                END), 0) AS TotalCurrentMonth,
                COALESCE(SUM(CASE
                    WHEN p.PurchaseDate >= DATE_TRUNC('month', NOW() - INTERVAL '1 month')
                         AND p.PurchaseDate < DATE_TRUNC('month', NOW())
                    THEN pi.TotalPrice ELSE 0
                END), 0) AS TotalLastMonth
            FROM PurchaseItems pi
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            INNER JOIN Products pr ON pr.Id = pi.ProductId
            INNER JOIN Categories c ON c.Id = pr.CategoryId
            WHERE p.UserId = @UserId
              AND pr.CategoryId IS NOT NULL
              AND p.PurchaseDate >= DATE_TRUNC('month', NOW() - INTERVAL '1 month')
            GROUP BY c.Id, c.Name
            HAVING
                COALESCE(SUM(CASE
                    WHEN p.PurchaseDate >= DATE_TRUNC('month', NOW())
                    THEN pi.TotalPrice ELSE 0
                END), 0) > 0
                OR
                COALESCE(SUM(CASE
                    WHEN p.PurchaseDate >= DATE_TRUNC('month', NOW() - INTERVAL '1 month')
                         AND p.PurchaseDate < DATE_TRUNC('month', NOW())
                    THEN pi.TotalPrice ELSE 0
                END), 0) > 0
            ORDER BY TotalCurrentMonth DESC, TotalLastMonth DESC";

        var categories = (await connection.QueryAsync<CategorySummaryDto>(categoriesSql, new { UserId = userId })).ToList();

        return Ok(new DashboardDto
        {
            TotalCurrentMonth = currentTotal,
            TotalLastMonth = lastTotal,
            Categories = categories
        });
    }
}
