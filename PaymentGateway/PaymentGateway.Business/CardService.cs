using AutoMapper;
using PaymentGateway.Domain.DomainObjects;
using PaymentGateway.Domain.DTOs;
using PaymentGateway.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Business
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private IMapper _mapper;
        public CardService(ICardRepository cardRepository, IMapper mapper)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
        }

        public async Task<Card> RetrieveCardByIdAndCvvAsync(Guid card_id, string cvv)
        {
            var savedCard = await _cardRepository.GetByIdAsync(card_id);

            //If no card found in db or CVV doesn't match return null
            if (savedCard == null || savedCard.Cvv != cvv) return null;

            return savedCard;
        }

        public async Task<Card> RetrieveCardByIdAsync(Guid card_id)
        {
            return await _cardRepository.GetByIdAsync(card_id);
        }

        public async Task<Card> SaveCardAsync(CardRequestDTO cardRequestDTO)
        {
            Card card = _mapper.Map<Card>(cardRequestDTO);
            var dbCard = await _cardRepository.CreateAsync(card);
            return dbCard;
        }
    }
}
