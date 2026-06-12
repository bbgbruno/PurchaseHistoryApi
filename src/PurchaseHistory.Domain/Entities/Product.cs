namespace PurchaseHistory.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string NormalizedName { get; set; }
        = string.Empty;

    public string? Brand { get; set; }

    public Guid? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }
}