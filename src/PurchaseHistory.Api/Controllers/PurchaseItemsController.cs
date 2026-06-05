using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/purchase-items")]
public class PurchaseItemsController : ControllerBase
{
    [HttpPatch("{id}/product-category")]
    public async Task<IActionResult> UpdateProductCategory(
        Guid id,
        [FromQuery] Guid userId,
        [FromBody] ProductCategoryRequest request,
        [FromServices] IPurchaseItemRepository purchaseItemRepository,
        [FromServices] IProductRepository productRepository)
    {
        var item = await purchaseItemRepository.GetByIdAsync(id, userId);

        if (item?.ProductId == null)
            return NotFound();

        await productRepository.UpdateCategoryAsync(item.ProductId.Value, request.CategoryId);
        return NoContent();
    }
}

public class ProductCategoryRequest
{
    public Guid? CategoryId { get; set; }
}
