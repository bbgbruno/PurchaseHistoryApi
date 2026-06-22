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

    [HttpPatch("{id}/discount")]
    public async Task<IActionResult> UpdateDiscount(
        Guid id,
        [FromQuery] Guid userId,
        [FromBody] DiscountRequest request,
        [FromServices] IPurchaseItemRepository purchaseItemRepository)
    {
        var item = await purchaseItemRepository.GetByIdAsync(id, userId);
        if (item == null)
            return NotFound();

        var newUnitPrice = item.UnitPrice - request.Discount;
        if (newUnitPrice < 0)
            newUnitPrice = 0;

        var newTotalPrice = newUnitPrice * item.Quantity;

        await purchaseItemRepository.UpdateDiscountAsync(id, request.Discount, newUnitPrice, newTotalPrice);
        return NoContent();
    }
}

public class DiscountRequest
{
    public decimal Discount { get; set; }
}

public class ProductCategoryRequest
{
    public Guid? CategoryId { get; set; }
}
