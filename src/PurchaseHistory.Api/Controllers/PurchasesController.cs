using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/purchases")]
public class PurchasesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid userId,
        [FromServices] IPurchaseRepository repository)
    {
        var purchases = await repository.GetAllAsync(userId);
        return Ok(purchases);
    }

    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItems(
        Guid id,
        [FromQuery] Guid userId,
        [FromServices] IPurchaseItemRepository repository)
    {
        var items = await repository.GetByPurchaseIdAsync(id, userId);
        return Ok(items);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromQuery] Guid userId,
        [FromServices] IPurchaseRepository repository)
    {
        await repository.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpPatch("{id}/purchase-date")]
    public async Task<IActionResult> UpdatePurchaseDate(
        Guid id,
        [FromQuery] Guid userId,
        [FromBody] UpdatePurchaseDateRequest request,
        [FromServices] IPurchaseRepository repository)
    {
        var updated = await repository.UpdatePurchaseDateAsync(id, request.PurchaseDate, userId);
        if (!updated)
            return NotFound();

        return NoContent();
    }
}

public class UpdatePurchaseDateRequest
{
    public DateTime PurchaseDate { get; set; }
}
