using PaymentGateway.Domain.DomainObjects;
using PaymentGateway.Domain.DTOs;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Business
{
    public interface ICardService
    {
        Task<Card> RetrieveCardByIdAndCvvAsync(Guid card_id, string cvv);
        Task<Card> RetrieveCardByIdAsync(Guid card_id);
        Task<Card> SaveCardAsync(CardRequestDTO cardRequestDTO);
    }
}