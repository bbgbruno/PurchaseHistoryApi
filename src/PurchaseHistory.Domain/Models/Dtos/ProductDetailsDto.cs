namespace PurchaseHistory.Domain.Dtos;

public class ProductDetailsDto
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public decimal LowestPrice { get; set; }

    public decimal HighestPrice { get; set; }

    public decimal AveragePrice { get; set; }

    public List<ProductHistoryDto> History { get; set; }
        = [];
}