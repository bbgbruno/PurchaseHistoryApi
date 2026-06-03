namespace PurchaseHistory.Domain.Dtos;

public class ProductHistoryDto
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal Quantity { get; set; }

    public string StoreName { get; set; }
        = string.Empty;

    public DateTime? PurchaseDate { get; set; }
}