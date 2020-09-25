using FluentValidation;
using PaymentGateway.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.Validation
{
    public class PaymentRequestDTOValidator : AbstractValidator<PaymentRequestDTO>
    {
        public PaymentRequestDTOValidator()
        {
            RuleFor(x => x.merchant_id).NotEmpty().WithMessage("merchant_id was not provided");
            
            RuleFor(x => x.Currency).NotEmpty().WithMessage("currency was not provided");
            RuleFor(x => x.Currency).Must(x=> x.Length == 3).WithMessage("Invalid currency Format. Please provide the 3 letter currency code").When(x=>!String.IsNullOrEmpty(x.Currency));
            
            RuleFor(x => x.Amount).NotEmpty().WithMessage("Amount was not provided");
            //This might require advance validation based on supported minimum transaction based on different currencies
            RuleFor(x => x.Amount).Must(x => x > 0).WithMessage("Amount should be greater than 0");
            
            RuleFor(x => x.Payment_Description).NotEmpty().WithMessage("payment_description was not provided");
            
            RuleFor(x => x.Card_Info).NotNull().WithMessage("card_info was not provided");
            RuleFor(x => x.Card_Info).SetValidator(new CardDTOValidator()).When(x=>x.Card_Info!=null);
        }
    }
}
