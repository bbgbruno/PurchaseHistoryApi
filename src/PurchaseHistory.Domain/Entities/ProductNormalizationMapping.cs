namespace PurchaseHistory.Domain.Entities;

public class ProductNormalizationMapping
{
    public Guid Id { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public string ReplacementText { get; set; } = string.Empty;
    public string MatchType { get; set; } = "Exact";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
