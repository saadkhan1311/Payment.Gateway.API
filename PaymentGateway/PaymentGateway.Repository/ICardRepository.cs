using PaymentGateway.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Repository
{
    public interface ICardRepository : IBaseRepository
    {
        Task<Card> CreateAsync(Card card_info);
        Task<Card> GetByIdAsync(Guid id);
    }
}
