using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/normalization/keywords")]
public class NormalizationKeywordsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IProductKeywordSubstitutionRepository repository)
    {
        var substitutions = await repository.GetAllActiveAsync();
        return Ok(substitutions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateKeywordRequest request,
        [FromServices] IProductKeywordSubstitutionRepository repository)
    {
        var substitution = new ProductKeywordSubstitution
        {
            OriginalTerm = request.OriginalTerm,
            ReplacementTerm = request.ReplacementTerm,
            IsActive = true
        };

        await repository.CreateAsync(substitution);
        return Ok(substitution);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] IProductKeywordSubstitutionRepository repository)
    {
        var substitution = await repository.GetByIdAsync(id);
        if (substitution == null)
            return NotFound();

        await repository.DeleteAsync(id);
        return NoContent();
    }
}

public class CreateKeywordRequest
{
    public string OriginalTerm { get; set; } = string.Empty;
    public string ReplacementTerm { get; set; } = string.Empty;
    
}