using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface ICouponImportRepository
{
    Task<Guid> CreateAsync(CouponImport couponImport);
    Task<IEnumerable<CouponImport>> GetPendingAsync();
    Task DeleteAsync(Guid id);
    Task UpdateStatusAsync(Guid id, string status);
}
