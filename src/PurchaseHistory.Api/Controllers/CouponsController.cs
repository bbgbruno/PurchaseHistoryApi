using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Application.UseCases.GetProducts;
using PurchaseHistory.Application.UseCases.UploadCouponHtml;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/coupons")]
public class CouponsController : ControllerBase
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

            using var reader =
                new StreamReader(file.OpenReadStream());

            var html =
                await reader.ReadToEndAsync();

            var input = new UploadCouponHtmlInput
            {
                HtmlContent = html
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
    public async Task<IActionResult> GetProducts( [FromServices] GetProductsUseCase useCase)
    {
        var output = await useCase.Handle();

        return Ok(output);
    }
}