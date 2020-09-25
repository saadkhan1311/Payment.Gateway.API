using PaymentGateway.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.DTOs
{
    public class AcquirerRequestDTO
    {
        public Card card { get; set; }

        public Guid Transaction_Reference_id { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public string Payment_Description { get; set; }
        /// <summary>
        /// This should be in reality, the account information of the merchant
        /// or if payment is credited first to checkout and then checkout credits afterwards to merchant the checkout's account info
        /// </summary>
        public string Registered_Account_Name { get; set; } = "Checkout Ltd";
    }
}
