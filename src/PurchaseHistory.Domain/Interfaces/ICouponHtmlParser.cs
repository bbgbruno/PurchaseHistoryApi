
using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Models.Dtos;

namespace PurchaseHistory.Domain.Interfaces
{
    public interface ICouponHtmlParser
    {
        ImportedCouponDto Parse(string html);
    }
}
