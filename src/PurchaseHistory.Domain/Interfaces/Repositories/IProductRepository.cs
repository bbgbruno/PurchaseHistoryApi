using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> FindByNameAsync(string normalizedName, Guid userId);

    Task<Guid> CreateAsync(Product product);

    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(Guid id);

    Task UpdateAsync(Product product);
    Task UpdateCategoryAsync(Guid id, Guid? categoryId);

    Task DeleteAsync(Guid id);
}