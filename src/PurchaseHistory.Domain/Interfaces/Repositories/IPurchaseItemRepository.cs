using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface IPurchaseItemRepository
{
    Task SaveAsync(IEnumerable<PurchaseItem> items);
    Task<IEnumerable<ProductSearchResultDto>> SearchProductsAsync(string term, Guid userId);
    Task<ProductDetailsDto?> GetProductDetailsAsync(Guid productId, Guid userId);
    Task RedirectProductIdAsync(Guid oldProductId, Guid newProductId);
    Task<IEnumerable<PurchaseItem>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<PurchaseItem>> GetByPurchaseIdAsync(Guid purchaseId, Guid userId);
    Task<PurchaseItem?> GetByIdAsync(Guid id, Guid userId);
}
