using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface IProductKeywordSubstitutionRepository
{
    Task<IEnumerable<ProductKeywordSubstitution>> GetAllActiveAsync();
    Task<ProductKeywordSubstitution?> GetByIdAsync(Guid id);
    Task CreateAsync(ProductKeywordSubstitution substitution);
    Task DeleteAsync(Guid id);
}
