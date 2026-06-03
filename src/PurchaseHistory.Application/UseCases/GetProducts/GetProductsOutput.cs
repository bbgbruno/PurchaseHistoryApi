using PurchaseHistory.Domain.Entities;

namespace PurchaseHistory.Application.UseCases.GetProducts;

public class GetProductsOutput
{
    public IEnumerable<Product> Products { get; set; }
        = Enumerable.Empty<Product>();
}