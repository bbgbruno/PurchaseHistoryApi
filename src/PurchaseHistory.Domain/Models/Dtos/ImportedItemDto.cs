namespace PurchaseHistory.Domain.Models.Dtos;

public class ImportedItemDto
{
    public string OriginalDescription { get; set; }
        = string.Empty;

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}