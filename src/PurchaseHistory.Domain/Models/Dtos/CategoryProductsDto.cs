namespace PurchaseHistory.Domain.Dtos;

public class CategoryProductsDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<CategoryProductItemDto> CurrentMonth { get; set; } = [];
    public List<CategoryProductItemDto> LastMonth { get; set; } = [];
}

public class CategoryProductItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
