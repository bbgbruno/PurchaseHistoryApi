namespace PurchaseHistory.Domain.Entities;

public class Purchase
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; } = Guid.Parse("36115116-5fb6-46a9-a42f-4d26e4f7f896");

    public Guid StoreId { get; set; }

    public string? AccessKey { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public decimal TotalValue { get; set; }

    public string? XmlFileName { get; set; }

    public string? XmlContent { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PurchaseItem> Items { get; set; } = [];
}