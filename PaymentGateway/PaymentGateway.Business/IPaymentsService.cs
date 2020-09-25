using PaymentGateway.Domain.DomainObjects;
using PaymentGateway.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Business
{
    public interface IPaymentsService
    {
        Task<PaymentResponseDTO> ProcessPaymentAsync(Guid transaction_reference, PaymentRequestDTO paymentRequest);
        Task<PaymentResponseDTO> GetPaymentByAcquirerRefrenceIdAsync(Guid acuirer_reference_id);
    }
}
