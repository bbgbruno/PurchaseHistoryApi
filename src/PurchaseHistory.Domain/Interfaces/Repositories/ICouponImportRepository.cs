using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface ICouponImportRepository
{
    Task<Guid> CreateAsync(CouponImport couponImport);
    Task<IEnumerable<CouponImport>> GetPendingAsync(Guid userId);
}
