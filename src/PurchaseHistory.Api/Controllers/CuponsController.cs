using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Api.Auth;
using PurchaseHistory.Application.UseCases.GetProducts;
using PurchaseHistory.Application.UseCases.UploadCouponHtml;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/cupons")]
[Authorize]
public class CuponsController : ControllerBase
{
    [HttpPost("upload-html")]
    public async Task<IActionResult> UploadHtml(
        IFormFile file,
        [FromServices] UploadCouponHtmlUseCase useCase)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    message = "Arquivo inválido."
                });
            }

            var userId = User.GetUserId();

            using var reader =
                new StreamReader(file.OpenReadStream());

            var html =
                await reader.ReadToEndAsync();

            var input = new UploadCouponHtmlInput
            {
                HtmlContent = html,
                UserId = userId
            };

            var output =
                await useCase.Handle(input);

            return Ok(output);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromServices] GetProductsUseCase useCase)
    {
        var output = await useCase.Handle();

        return Ok(output);
    }

    [HttpGet("imports/pending")]
    public async Task<IActionResult> GetPending(
        [FromServices] ICouponImportRepository repository)
    {
        var userId = User.GetUserId();
        var items = await repository.GetPendingAsync(userId);
        return Ok(items);
    }

    [HttpPost("imports")]
    public async Task<IActionResult> CreateImport(
        [FromBody] CreateCouponImportRequest request,
        [FromServices] ICouponImportRepository repository)
    {
        var userId = User.GetUserId();
        var couponImport = new CouponImport
        {
            UserId = userId,
            AccessKey = request.AccessKey
        };

        var id = await repository.CreateAsync(couponImport);

        return Ok(new { id, couponImport.AccessKey, couponImport.Status });
    }
}

public class CreateCouponImportRequest
{
    public string AccessKey { get; set; } = string.Empty;
}
