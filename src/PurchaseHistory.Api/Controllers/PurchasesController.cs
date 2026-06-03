using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/purchases")]
public class PurchasesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IPurchaseRepository repository)
    {
        var purchases = await repository.GetAllAsync();
        return Ok(purchases);
    }
}
