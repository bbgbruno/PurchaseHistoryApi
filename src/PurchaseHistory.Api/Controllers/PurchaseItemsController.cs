using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Api.Auth;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/purchase-items")]
[Authorize]
public class PurchaseItemsController : ControllerBase
{
    [HttpPatch("{id}/product-category")]
    public async Task<IActionResult> UpdateProductCategory(
        Guid id,
        [FromBody] ProductCategoryRequest request,
        [FromServices] IPurchaseItemRepository purchaseItemRepository,
        [FromServices] IProductRepository productRepository)
    {
        var userId = User.GetUserId();
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
