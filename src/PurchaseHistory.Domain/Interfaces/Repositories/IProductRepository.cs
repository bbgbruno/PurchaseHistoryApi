using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> FindByNameAsync(string normalizedName);

    Task<Guid> CreateAsync(Product product);

    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(Guid id);

    Task UpdateAsync(Product product);

    Task DeleteAsync(Guid id);
}