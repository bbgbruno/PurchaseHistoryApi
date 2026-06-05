using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Application.UseCases.GetProductDetails;

public class GetProductDetailsUseCase
{
    private readonly IPurchaseItemRepository
        _repository;

    public GetProductDetailsUseCase(
        IPurchaseItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDetailsDto?>
        Handle(Guid productId, Guid userId)
    {
        return await _repository
            .GetProductDetailsAsync(productId, userId);
    }
}
