using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PaymentGateway.Domain.DomainObjects
{
    public class PaymentResponse
    {
        [Key]
        public Guid Transaction_Reference_Id { get; set; }

        public Guid Acquirer_Reference_Id { get; set; }

        public string Currency { get; set; }
        public double Amount { get; set; }
        
        /// <summary>
        /// Foreign key to the Card table. For simplicity relations were not added
        /// </summary>
        public Guid Card_id { get; set; }

        public DateTime Processed_On { get; set; }

        public int Status { get; set; }
    }
}
