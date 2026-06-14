using PurchaseHistory.Domain.Models.Dtos;

namespace PurchaseHistory.Domain.Interfaces;

public interface IPdfCouponParser
{
    ImportedCouponDto Parse(byte[] pdfContent);
}
