using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.DTOs
{
    public class CardResponseDTO
    {
        private string masked_card_number;

        public CardResponseDTO(string card_number)
        {
            MaskCardNumber(card_number);
        }
        public Guid Id { get; set; }
        public string Number { get => masked_card_number; }
        public string Expiry_Month { get; set; }
        public string Expiry_Year { get; set; }

        private void MaskCardNumber(string unmasked_card_number)
        {
            masked_card_number = "XXXXXXXXXXXX";
            if (!String.IsNullOrEmpty(unmasked_card_number) && unmasked_card_number.Length == 16)
                masked_card_number = masked_card_number + unmasked_card_number.Remove(0, 12);           
        }
    }
}
