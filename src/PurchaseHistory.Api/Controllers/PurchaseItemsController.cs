using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/purchase-items")]
public class PurchaseItemsController : ControllerBase
{
    [HttpPatch("{id}/category")]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] ItemCategoryRequest request,
        [FromServices] IPurchaseItemRepository repository)
    {
        await repository.UpdateCategoryAsync(id, request.CategoryId);
        return NoContent();
    }
}

public class ItemCategoryRequest
{
    public Guid? CategoryId { get; set; }
}
