using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Application.UseCases.ApplyProductNormalization;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/normalization/names")]
public class NormalizationNameController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IProductNormalizationMappingRepository repository)
    {
        var mappings = await repository.GetAllActiveAsync();
        return Ok(mappings);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMappingRequest request,
        [FromServices] IProductNormalizationMappingRepository repository)
    {
        var mapping = new ProductNormalizationMapping
        {
            OriginalText = request.OriginalText,
            ReplacementText = request.ReplacementText,
            MatchType = request.MatchType ?? "Exact",
            IsActive = true
        };

        await repository.CreateAsync(mapping);
        return Ok(mapping);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] IProductNormalizationMappingRepository repository)
    {
        var mapping = await repository.GetByIdAsync(id);
        if (mapping == null)
            return NotFound();

        await repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("apply")]
    public async Task<IActionResult> Apply(
        [FromServices] ApplyProductNormalizationUseCase useCase)
    {
        var result = await useCase.Handle();
        return Ok(result);
    }
}

public class CreateMappingRequest
{
    public string OriginalText { get; set; } = string.Empty;
    public string ReplacementText { get; set; } = string.Empty;
    public string? MatchType { get; set; }
}
