using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.Domain;
using PaymentGateway.Domain.DomainObjects;
using PaymentGateway.Domain.DTOs;
using PaymentGateway.Repository;

namespace PaymentGateway.Business
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IRequestInfo _requestInfo;
        private readonly IAcquirerService _acquirerService;
        private readonly ICardService _cardService;
        private readonly IPaymentResponseRepository _paymentResponseRepository;
        private IMapper _mapper;

        public PaymentsService(IRequestInfo requestInfo, IMapper mapper,
            IPaymentResponseRepository paymentResponseRepository,
            IAcquirerService acquirerService, ICardService cardService)
        {
            _requestInfo = requestInfo;
            _acquirerService = acquirerService;
            _cardService = cardService;
            _paymentResponseRepository = paymentResponseRepository;
            _mapper = mapper;
        }

        public async Task<PaymentResponseDTO> GetPaymentByAcquirerRefrenceIdAsync(Guid acuirer_reference_id)
        {
            PaymentResponse paymentResponse = await _paymentResponseRepository.GetByAcquirerRefrenceIdAsync(acuirer_reference_id);

            PaymentResponseDTO paymentResponseDTO = _mapper.Map<PaymentResponseDTO>(paymentResponse);
            if (paymentResponse.Card_id != Guid.Empty)
            {
                Card card = await _cardService.RetrieveCardByIdAsync(paymentResponse.Card_id);

                if (card != null)
                {
                    CardResponseDTO cardResponseDTO = new CardResponseDTO(card.Number);
                    cardResponseDTO.Id = card.Id;
                    cardResponseDTO.Expiry_Month = card.Expiry_Month;
                    cardResponseDTO.Expiry_Year = card.Expiry_Year;
                    paymentResponseDTO.Card_Info = cardResponseDTO;
                }
            }
            return paymentResponseDTO;
        }

        public async Task<PaymentResponseDTO> ProcessPaymentAsync(Guid transaction_reference, PaymentRequestDTO paymentRequest)
        {
            PaymentResponseDTO paymentResponseDTO = _mapper.Map<PaymentResponseDTO>(paymentRequest);
            paymentResponseDTO.Transaction_Reference_Id = transaction_reference;
            try
            {
                #region Save/Retrieve Card
                Card card_to_use;

                //An Existing/Previously used card was provided so, retrieve from db
                if (paymentRequest.Card_Info.Id.HasValue && paymentRequest.Card_Info.Id != Guid.Empty)
                {
                    card_to_use = await _cardService.RetrieveCardByIdAndCvvAsync(paymentRequest.Card_Info.Id.Value, paymentRequest.Card_Info.Cvv);
                    if (card_to_use == null)
                    {
                        paymentResponseDTO.Status = StatusCode.INVALID_DATA;
                        paymentResponseDTO.Errors = new List<string>() { "Could not retrieve card, Invalid card id or cvv was provided" };
                        return paymentResponseDTO;
                    }
                }
                //A new card was provided so save to database (Not validating if the card exisits as it could be that another merchant saved the same user's card)
                else
                {
                    card_to_use = await _cardService.SaveCardAsync(paymentRequest.Card_Info);
                }

                CardResponseDTO cardResponseDTO = new CardResponseDTO(card_to_use.Number);
                cardResponseDTO.Id = card_to_use.Id;
                cardResponseDTO.Expiry_Month = card_to_use.Expiry_Month;
                cardResponseDTO.Expiry_Year = card_to_use.Expiry_Year;
                paymentResponseDTO.Card_Info = cardResponseDTO;
                #endregion

                #region Acquirer request and response
                //Call the Acquiring Bank Service, set the acquirer_reference_id
                AcquirerRequestDTO acquirerRequestDTO = new AcquirerRequestDTO();
                acquirerRequestDTO.Transaction_Reference_id = transaction_reference;
                acquirerRequestDTO.Amount = paymentRequest.Amount;
                acquirerRequestDTO.Currency = paymentRequest.Currency;
                acquirerRequestDTO.card = card_to_use;
                acquirerRequestDTO.Payment_Description = paymentRequest.Payment_Description;

                AcquirerResponseDTO acquirerResponseDTO = _acquirerService.ForwardPaymentRequestToAcquirer(acquirerRequestDTO);

                paymentResponseDTO.Acquirer_Reference_Id = acquirerResponseDTO.Payment_Reference_Id;
                paymentResponseDTO.Status = acquirerResponseDTO.Status;
                paymentResponseDTO.Processed_On = acquirerResponseDTO.Processed_On;
                #endregion

                //Save Payment Response
                await SavePaymentResponseAsync(paymentResponseDTO,card_to_use);
                return paymentResponseDTO;
            }
            catch (Exception ex)
            {
                //Log the exception
                paymentResponseDTO.Errors = new List<string>() { StringConstants.UNEXPECTED_ERROR_MESSAGE };
                paymentResponseDTO.Status = Domain.DTOs.StatusCode.UNKNOWN;
            }
            return paymentResponseDTO;
        }

        private async Task<PaymentResponse> SavePaymentResponseAsync(PaymentResponseDTO paymentResponseDTO, Card card_used)
        {
            PaymentResponse paymentResponse = _mapper.Map<PaymentResponse>(paymentResponseDTO);
            paymentResponse.Card_id = card_used.Id;
            return await _paymentResponseRepository.CreateAsync(paymentResponse);
        }
    }
}
