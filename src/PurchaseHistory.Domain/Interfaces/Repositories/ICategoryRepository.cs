using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync(Guid userId);
    Task<Category?> GetByIdAsync(Guid id, Guid userId);
    Task<Guid> CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id, Guid userId);
}
