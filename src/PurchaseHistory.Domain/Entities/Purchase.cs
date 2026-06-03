namespace PurchaseHistory.Domain.Entities;

public class Purchase
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; } = Guid.Parse("8EB78D4D-9744-49F6-9C34-FE0774E322FB");

    public Guid StoreId { get; set; }

    public string? AccessKey { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public decimal TotalValue { get; set; }

    public string? XmlFileName { get; set; }

    public string? XmlContent { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PurchaseItem> Items { get; set; } = [];
}