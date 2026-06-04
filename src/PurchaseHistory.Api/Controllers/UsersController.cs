using Microsoft.AspNetCore.Mvc;
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetAll(
        [FromServices] IUserRepository repository)
    {
        var users = await repository.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] IUserRepository repository)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        [FromServices] IUserRepository repository)
    {
        var existing = await repository.GetByEmailAsync(request.Email);
        if (existing != null)
            return Conflict(new { message = "E-mail já cadastrado." });

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true
        };

        var id = await repository.CreateAsync(user);
        user.Id = id;

        return CreatedAtAction(nameof(GetById), new { id }, user);
    }

    [HttpPut("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        [FromServices] IUserRepository repository)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await repository.GetByEmailAsync(request.Email);
            if (existing != null)
                return Conflict(new { message = "E-mail já cadastrado." });
        }

        user.Name = request.Name;
        user.Email = request.Email;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        await repository.UpdateAsync(user);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] IUserRepository repository)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        await repository.DeleteAsync(id);
        return NoContent();
    }
}

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public bool IsActive { get; set; }
}
