namespace PurchaseHistory.Application.UseCases.UploadCouponHtml;

public class UploadCouponHtmlOutput
{
    public Guid PurchaseId { get; set; }

    public string StoreName { get; set; }
        = string.Empty;

    public DateTime? PurchaseDate { get; set; }

    public int TotalItems { get; set; }

    public decimal TotalValue { get; set; }
}