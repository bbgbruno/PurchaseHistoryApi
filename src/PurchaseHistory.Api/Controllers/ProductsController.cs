using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Api.Auth;
using PurchaseHistory.Application.UseCases.GetProductDetails;
using PurchaseHistory.Application.UseCases.SearchProducts;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string term,
        [FromServices] SearchProductsUseCase useCase)
    {
        var userId = User.GetUserId();
        var result = await useCase.Handle(term, userId);

        return Ok(result);
    }

    [HttpGet("history/{productId}")]
    public async Task<IActionResult> GetDetails(
        Guid productId,
        [FromServices] GetProductDetailsUseCase useCase)
    {
        var userId = User.GetUserId();
        var result = await useCase.Handle(productId, userId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
