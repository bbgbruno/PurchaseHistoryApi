namespace PurchaseHistory.Domain.Entities;

public class ProductKeywordSubstitution
{
    public Guid Id { get; set; }
    public string OriginalTerm { get; set; } = string.Empty;
    public string ReplacementTerm { get; set; } = string.Empty;
    public string MatchType { get; set; } = "WholeWord";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
