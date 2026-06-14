namespace PurchaseHistory.Application.UseCases.UploadCouponPdf;

public class UploadPdfCouponInput
{
    public byte[] FileContent { get; set; } = [];
    public Guid UserId { get; set; }
}
