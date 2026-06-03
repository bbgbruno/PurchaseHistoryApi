using PurchaseHistory.Domain.Entities;


namespace PurchaseHistory.Domain.Interfaces
{
    public interface IXmlInvoiceParser
    {
        Purchase Parse(string xmlContent);
    }
}
