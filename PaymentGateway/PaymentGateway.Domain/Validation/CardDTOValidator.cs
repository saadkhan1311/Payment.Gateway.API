using FluentValidation;
using FluentValidation.Results;
using PaymentGateway.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.Validation
{
    public class CardDTOValidator : AbstractValidator<CardRequestDTO>
    {
        public CardDTOValidator()
        {
            
            RuleFor(x => x.Cvv).NotEmpty().WithMessage("Cvv was not provided");
            //CVV can be 3 digits mostly but for Amex it can be 4 digits too. 
            //We can aditionally add validation if it's numeric if we are sure that CVV can only be numeric
            RuleFor(x => x.Cvv).Must(x=> x.Length == 3 || x.Length ==4).WithMessage("Cvv was not provided").When(x=> !String.IsNullOrEmpty(x.Cvv));
            
            RuleFor(x => x.Number).NotEmpty().Must(x => x.Length == 16).WithMessage("Number should be 16 digit card number").When(x => x.Id == null || x.Id == Guid.Empty);

            // Check if Card is not expired and provided month and year values are valid (Only if Id ws not provided)
            RuleFor(x => x).Custom((dto,context) =>
              {
                  if (dto.Id == null || dto.Id == Guid.Empty)
                  {
                      int year = 0;
                      if (Int32.TryParse(dto.Expiry_Year, out year))
                      {
                          if (year >= DateTime.UtcNow.Year)
                          {
                              int month = 0;
                              if (Int32.TryParse(dto.Expiry_Month, out month) && month > 0 && month <= 12)
                              {
                                  if (year == DateTime.UtcNow.Year)
                                  {
                                      if (month < DateTime.UtcNow.Month)
                                      {
                                          context.AddFailure(new ValidationFailure(nameof(dto.Expiry_Year), "The provided card is expired"));
                                      }
                                  }
                                  else
                                      return;
                              }
                              else
                              {
                                  context.AddFailure(new ValidationFailure(nameof(dto.Expiry_Month), "Expiry month should be between 01 to 12"));
                              }
                          }
                          else
                          {
                              context.AddFailure(new ValidationFailure(nameof(dto.Expiry_Year), "The provided card is expired"));
                          }

                      }
                      else
                      {
                          context.AddFailure(new ValidationFailure(nameof(dto.Expiry_Year), "expiry_year was not provided or was not in the correct format"));
                      }
                  }
              });
        }
    }
}
