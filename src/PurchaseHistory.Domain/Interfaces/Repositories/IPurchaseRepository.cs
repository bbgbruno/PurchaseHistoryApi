using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Domain.Interfaces.Repositories;

public interface IPurchaseRepository
{
    Task<Guid> CreateAsync(Purchase purchase);
}