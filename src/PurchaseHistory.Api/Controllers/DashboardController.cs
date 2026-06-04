using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Api.Auth;
using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Infrastructure.Data;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] DbConnectionFactory connectionFactory)
    {
        var userId = User.GetUserId();

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
