using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Application.UseCases.UploadCouponHtml;
using PurchaseHistory.Application.UseCases.UploadCouponPdf;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/cupons")]
public class CuponsController : ControllerBase
{
    [HttpPost("upload-pdf")]
    public async Task<IActionResult> UploadPdf(
        IFormFile file,
        [FromQuery] Guid userId,
        [FromServices] UploadPdfCouponUseCase useCase)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Arquivo inválido." });

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var input = new UploadPdfCouponInput
            {
                FileContent = ms.ToArray(),
                UserId = userId
            };

            var output = await useCase.Handle(input);
            return Ok(output);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("upload-html")]
    public async Task<IActionResult> UploadHtml(
        IFormFile file,
        [FromQuery] Guid userId,
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


    [HttpGet("imports/pending")]
    public async Task<IActionResult> GetPending([FromServices] ICouponImportRepository repository)
    {
        var items = await repository.GetPendingAsync();
        return Ok(items);
    }

    [HttpPost("imports")]
    public async Task<IActionResult> CreateImport(
        [FromBody] CreateCouponImportRequest request,
        [FromServices] ICouponImportRepository repository)
    {
        var couponImport = new CouponImport
        {
            UserId = request.UserId,
            AccessKey = request.AccessKey
        };

        var id = await repository.CreateAsync(couponImport);

        return Ok(new { id, couponImport.AccessKey, couponImport.Status });
    }

    [HttpDelete("imports/{id}")]
    public async Task<IActionResult> DeleteImport(
        Guid id,
        [FromServices] ICouponImportRepository repository)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("imports/{id}/status")]
    public async Task<IActionResult> UpdateImportStatus(
        Guid id,
        [FromBody] UpdateImportStatusRequest request,
        [FromServices] ICouponImportRepository repository)
    {
        await repository.UpdateStatusAsync(id, request.Status);
        return NoContent();
    }
}

public class CreateCouponImportRequest
{
    public Guid UserId { get; set; }
    public string AccessKey { get; set; } = string.Empty;
}

public class UpdateImportStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
