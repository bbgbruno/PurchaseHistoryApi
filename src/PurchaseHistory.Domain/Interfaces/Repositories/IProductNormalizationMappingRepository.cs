using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface IProductNormalizationMappingRepository
{
    Task<IEnumerable<ProductNormalizationMapping>> GetAllActiveAsync();
    Task<ProductNormalizationMapping?> GetByIdAsync(Guid id);
    Task CreateAsync(ProductNormalizationMapping mapping);
    Task UpdateAsync(ProductNormalizationMapping mapping);
    Task DeleteAsync(Guid id);
}
