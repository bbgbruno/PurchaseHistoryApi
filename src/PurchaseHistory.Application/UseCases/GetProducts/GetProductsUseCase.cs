using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;

namespace PurchaseHistory.Application.UseCases.GetProducts;

public class GetProductsUseCase
{
    private readonly IProductRepository _repository;

    public GetProductsUseCase(
        IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetProductsOutput> Handle()
    {
        var products = await _repository.FindByNameAsync("");

        return new GetProductsOutput
        {
            Products = [products]
        };
    }
}