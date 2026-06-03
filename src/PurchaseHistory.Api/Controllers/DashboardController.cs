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

        var totalSql = @"
            SELECT COALESCE(SUM(TotalValue), 0)
            FROM Purchases
            WHERE UserId = @UserId
              AND PurchaseDate >= NOW() - INTERVAL '30 days'";

        var total = await connection.ExecuteScalarAsync<decimal>(totalSql, new { UserId = userId });

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
              AND p.PurchaseDate >= NOW() - INTERVAL '30 days'
              AND pr.CategoryId IS NOT NULL
            GROUP BY c.Id, c.Name
            ORDER BY TotalSpent DESC";

        var categories = await connection.QueryAsync<CategorySummaryDto>(categoriesSql, new { UserId = userId });

        return Ok(new DashboardDto
        {
            TotalLastMonth = total,
            Categories = categories.ToList()
        });
    }
}
