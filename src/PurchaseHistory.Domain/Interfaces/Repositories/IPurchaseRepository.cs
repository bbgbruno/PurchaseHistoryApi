using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface IPurchaseRepository
{
    Task<Guid> CreateAsync(Purchase purchase);
    Task<IEnumerable<PurchaseListDto>> GetAllAsync();
    Task DeleteAsync(Guid id);
    Task<bool> ExistsByAccessKeyAsync(string accessKey);
}