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
        [FromServices] SearchProductsUseCase useCase)
    {
        var result =
            await useCase.Handle(term);

        return Ok(result);
    }


    [HttpGet("history/{productId}")]
    public async Task<IActionResult> GetDetails(Guid productId, [FromServices] GetProductDetailsUseCase useCase)
    {
        var result =
            await useCase.Handle(productId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}