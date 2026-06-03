using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid userId,
        [FromServices] ICategoryRepository repository)
    {
        var items = await repository.GetAllAsync(userId);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromQuery] Guid userId,
        [FromServices] ICategoryRepository repository)
    {
        var item = await repository.GetByIdAsync(id, userId);
        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        [FromServices] ICategoryRepository repository)
    {
        var category = new Category
        {
            Name = request.Name,
            UserId = request.UserId
        };

        var id = await repository.CreateAsync(category);
        category.Id = id;

        return CreatedAtAction(nameof(GetById), new { id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        [FromServices] ICategoryRepository repository)
    {
        var category = await repository.GetByIdAsync(id, request.UserId);
        if (category == null)
            return NotFound();

        category.Name = request.Name;
        await repository.UpdateAsync(category);

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromQuery] Guid userId,
        [FromServices] ICategoryRepository repository)
    {
        await repository.DeleteAsync(id, userId);
        return NoContent();
    }
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
