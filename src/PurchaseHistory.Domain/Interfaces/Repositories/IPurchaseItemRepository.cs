using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseHistory.Domain.Interfaces.Repositories
{
    public interface IPurchaseItemRepository
    {
        Task SaveAsync(IEnumerable<PurchaseItem> items);
        Task<IEnumerable<ProductSearchResultDto>> SearchProductsAsync(string term);
        Task<ProductDetailsDto?> GetProductDetailsAsync(Guid productId);
        Task RedirectProductIdAsync(Guid oldProductId, Guid newProductId);
        Task<IEnumerable<PurchaseItem>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<PurchaseItem>> GetByPurchaseIdAsync(Guid purchaseId);
    }
}
