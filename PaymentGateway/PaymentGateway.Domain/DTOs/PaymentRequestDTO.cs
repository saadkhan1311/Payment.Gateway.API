using AutoMapper;
using PaymentGateway.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.DTOs
{
    public class PaymentRequestDTO
    {
        public Guid merchant_id { get; set; }
        public CardRequestDTO Card_Info { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public string Payment_Description { get; set; }
    }
}
