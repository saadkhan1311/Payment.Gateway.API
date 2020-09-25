using PaymentGateway.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Business
{
    public class AcquiringBankService:IAcquirerService
    {
        public AcquirerResponseDTO ForwardPaymentRequestToAcquirer(AcquirerRequestDTO acquirerRequestDTO)
        {
            //here should be the actual code to call acquirer
            return null;
        }
    }
}
