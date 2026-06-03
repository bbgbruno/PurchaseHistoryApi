namespace PurchaseHistory.Domain.Entities;

public class CouponImport
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccessKey { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
