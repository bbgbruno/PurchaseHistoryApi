using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/coupon-imports")]
public class CouponImportsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCouponImportRequest request,
        [FromServices] ICouponImportRepository repository)
    {
        var couponImport = new CouponImport
        {
            UserId = request.UserId,
            AccessKey = request.AccessKey
        };

        var id = await repository.CreateAsync(couponImport);

        return Ok(new { id, couponImport.AccessKey, couponImport.Status });
    }
}

public class CreateCouponImportRequest
{
    public Guid UserId { get; set; }
    public string AccessKey { get; set; } = string.Empty;
}
