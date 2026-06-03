using PurchaseHistory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace PurchaseHistory.Domain.Interfaces.Repositories
{
    public interface IStoreRepository
    {
        Task<Store?> GetByDocumentAsync(string document);

        Task<Guid> CreateAsync(Store store);
    }
}
