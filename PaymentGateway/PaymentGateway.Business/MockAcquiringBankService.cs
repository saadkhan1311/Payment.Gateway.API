using PaymentGateway.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PaymentGateway.Business
{
    public class MockAcquiringBankService: IAcquirerService
    {
        public AcquirerResponseDTO ForwardPaymentRequestToAcquirer(AcquirerRequestDTO acquirerRequestDTO)
        {
            //Waiting for 1 second to mimic the wait which acquiring bank woul d actually take
            Thread.Sleep(1000);
            AcquirerResponseDTO acquirerResponseDTO = new AcquirerResponseDTO();
            acquirerResponseDTO.Transaction_Reference_id = acquirerRequestDTO.Transaction_Reference_id;
            acquirerResponseDTO.Payment_Reference_Id = Guid.NewGuid();
            acquirerResponseDTO.Status = StatusCode.APPROVED;
            acquirerResponseDTO.Processed_On = DateTime.UtcNow;
            return acquirerResponseDTO;
        }
    }
}
