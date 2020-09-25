using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PaymentGateway.Domain.DomainObjects
{
    public class Merchant
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Encrypted secret key for authentication
        /// </summary>
        [Required]
        public string Secret_Key { get; set; }

       
        // The bank account of the merchant to which processed payments will be transferred after receipt
        // of funds from acquiring bank
        // NOTE: The implementation has not been done keeping in mind the scope of the assignment
       
        //public Account Account_info { get; set; }
    }
}
