namespace PurchaseHistory.Application.UseCases.UploadCouponHtml;

public class UploadCouponHtmlInput
{
    public string HtmlContent { get; set; }
        = string.Empty;

    public string FileName { get; set; }
        = string.Empty;

    public Guid UserId { get; set; }
}