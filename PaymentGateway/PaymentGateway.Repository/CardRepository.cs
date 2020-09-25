using PaymentGateway.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace PaymentGateway.Repository
{
    public class CardRepository : BaseRepository<AppDbContext>, ICardRepository
    {
        private DbSet<Card> _cards;
        private readonly IDataProtector _protector;
        public CardRepository(AppDbContext dataContext, IDataProtectionProvider protectionProvider) : base(dataContext)
        {
            this._cards = dataContext.Cards;
            _protector = protectionProvider.CreateProtector("card_encryption_key_6187039c-c49a-49bc-8743-ee42efca1e5f");
        }

        public async Task<Card> CreateAsync(Card card_info)
        {
            Card newCard = new Card();
            //Encryption
            newCard.Cvv = _protector.Protect(card_info.Cvv);
            newCard.Number = _protector.Protect(card_info.Number);

            newCard.Expiry_Month = card_info.Expiry_Month;
            newCard.Expiry_Year = card_info.Expiry_Year;
            newCard.Card_Holder_Name = card_info.Card_Holder_Name;
            
            await _cards.AddAsync(newCard);
            await SaveChangesAsync();
            card_info.Id = newCard.Id;
            return card_info;
        }

        public async Task<Card> GetByIdAsync(Guid id)
        {
            var card = await _cards.FirstOrDefaultAsync(x => x.Id == id);
            if (card == null) return null;
            //Decryption
            card.Cvv = _protector.Unprotect(card.Cvv);
            card.Number = _protector.Unprotect(card.Number);
            return card;
        }
    }
}
