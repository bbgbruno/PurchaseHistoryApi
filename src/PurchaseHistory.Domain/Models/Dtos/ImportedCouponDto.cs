namespace PurchaseHistory.Domain.Models.Dtos;

public class ImportedCouponDto
{
    public string StoreName { get; set; } = string.Empty;

    public string DocumentNumber { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public DateTime? PurchaseDate { get; set; }

    public decimal TotalValue { get; set; }

    public List<ImportedItemDto> Items { get; set; }
        = [];
}