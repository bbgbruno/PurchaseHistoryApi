namespace PurchaseHistory.Domain.Dtos;

public class DashboardDto
{
    public decimal TotalLastMonth { get; set; }
    public List<CategorySummaryDto> Categories { get; set; } = [];
}

public class CategorySummaryDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
}
