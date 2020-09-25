using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.DTOs
{
    public class AcquirerResponseDTO
    {
        public Guid Payment_Reference_Id { get; set; }

        public Guid Transaction_Reference_id { get; set; }
        public StatusCode Status { get; set; }
        public DateTime Processed_On { get; set; }
    }
}
