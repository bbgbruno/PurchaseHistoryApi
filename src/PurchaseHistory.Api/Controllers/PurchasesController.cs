using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Api.Auth;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/purchases")]
[Authorize]
public class PurchasesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IPurchaseRepository repository)
    {
        var userId = User.GetUserId();
        var purchases = await repository.GetAllAsync(userId);
        return Ok(purchases);
    }

    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItems(
        Guid id,
        [FromServices] IPurchaseItemRepository repository)
    {
        var userId = User.GetUserId();
        var items = await repository.GetByPurchaseIdAsync(id, userId);
        return Ok(items);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] IPurchaseRepository repository)
    {
        var userId = User.GetUserId();
        await repository.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpPatch("{id}/purchase-date")]
    public async Task<IActionResult> UpdatePurchaseDate(
        Guid id,
        [FromBody] UpdatePurchaseDateRequest request,
        [FromServices] IPurchaseRepository repository)
    {
        var userId = User.GetUserId();
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
