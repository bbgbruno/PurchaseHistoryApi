namespace PurchaseHistory.Domain.Dtos;

public class ProductSearchResultDto
{
    public Guid ProductId { get; set; }
    public Guid PurchaseItemId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public string StoreName { get; set; }
        = string.Empty;
}