namespace PurchaseHistory.Domain.Entities;

public class PurchaseItem
{
    public Guid Id { get; set; }

    public Guid PurchaseId { get; set; }

    public Guid? ProductId { get; set; }

    public string OriginalDescription { get; set; } = string.Empty;

    public string? ProductCode { get; set; }

    public string? NcmCode { get; set; }

    public string? Ean { get; set; }

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }
}