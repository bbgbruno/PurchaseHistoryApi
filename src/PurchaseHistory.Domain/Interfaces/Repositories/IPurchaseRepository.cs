using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface IPurchaseRepository
{
    Task<Guid> CreateAsync(Purchase purchase);
    Task<IEnumerable<PurchaseListDto>> GetAllAsync(Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<bool> ExistsByAccessKeyAsync(string accessKey);
    Task<bool> UpdatePurchaseDateAsync(Guid id, DateTime purchaseDate, Guid userId);
}
