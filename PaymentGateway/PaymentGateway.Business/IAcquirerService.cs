using PaymentGateway.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Business
{
    public interface IAcquirerService
    {
        AcquirerResponseDTO ForwardPaymentRequestToAcquirer(AcquirerRequestDTO acquirerRequestDTO);
    }
}
