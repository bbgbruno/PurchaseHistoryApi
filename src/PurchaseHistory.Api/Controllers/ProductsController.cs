using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Application.UseCases.GetProductDetails;
using PurchaseHistory.Application.UseCases.SearchProducts;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string term,
        [FromQuery] Guid userId,
        [FromServices] SearchProductsUseCase useCase)
    {
        var result = await useCase.Handle(term, userId);

        return Ok(result);
    }

    [HttpGet("history/{productId}")]
    public async Task<IActionResult> GetDetails(
        Guid productId,
        [FromQuery] Guid userId,
        [FromServices] GetProductDetailsUseCase useCase)
    {
        var result = await useCase.Handle(productId, userId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
