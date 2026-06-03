using PurchaseHistory.Domain.Dtos;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Application.UseCases.SearchProducts;

public class SearchProductsUseCase
{
    private readonly IPurchaseItemRepository
        _repository;

    public SearchProductsUseCase(
        IPurchaseItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductSearchResultDto>>
        Handle(string term)
    {
        return await _repository
            .SearchProductsAsync(term);
    }
}