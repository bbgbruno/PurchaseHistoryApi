namespace PurchaseHistory.Domain.Dtos;

public class PurchaseListDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal TotalValue { get; set; }
    public string StoreName { get; set; } = string.Empty;
}
