using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/purchases")]
public class PurchasesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? userId,
        [FromServices] IPurchaseRepository repository)
    {
        var purchases = await repository.GetAllAsync(userId);
        return Ok(purchases);
    }

    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItems(
        Guid id,
        [FromServices] IPurchaseItemRepository repository)
    {
        var items = await repository.GetByPurchaseIdAsync(id);
        return Ok(items);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] IPurchaseRepository repository)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }
}
