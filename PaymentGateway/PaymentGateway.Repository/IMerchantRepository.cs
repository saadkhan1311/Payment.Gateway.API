using PaymentGateway.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Repository
{
    public interface IMerchantRepository : IBaseRepository
    {
        IEnumerable<Merchant> GetAll();
        Task<Merchant> AddAsync(Merchant merchant);
        Task<Guid> GetMerchantIdBySecretKey(string key);
    }
}
