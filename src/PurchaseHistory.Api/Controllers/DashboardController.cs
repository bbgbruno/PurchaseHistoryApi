using Dapper;
using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
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
                COALESCE(SUM(pi.TotalPrice), 0) AS TotalSpent
            FROM PurchaseItems pi
            INNER JOIN Purchases p ON p.Id = pi.PurchaseId
            INNER JOIN Products pr ON pr.Id = pi.ProductId
            INNER JOIN Categories c ON c.Id = pr.CategoryId
            WHERE p.UserId = @UserId
              AND p.PurchaseDate >= DATE_TRUNC('month', NOW())
              AND pr.CategoryId IS NOT NULL
            GROUP BY c.Id, c.Name
            ORDER BY TotalSpent DESC";

        var categories = await connection.QueryAsync<CategorySummaryDto>(categoriesSql, new { UserId = userId });

        return Ok(new DashboardDto
        {
            TotalCurrentMonth = currentTotal,
            TotalLastMonth = lastTotal,
            Categories = categories.ToList()
        });
    }
}
