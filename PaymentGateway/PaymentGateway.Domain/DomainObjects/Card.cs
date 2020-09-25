using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PaymentGateway.Domain.DomainObjects
{
   
    public class Card
    {
        public Card()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        [StringLength(2)]
        public string Expiry_Month { get; set; }
        [Required]
        [StringLength(4)]
        public string Expiry_Year { get; set; }
        [Required]
        public string Cvv { get; set; }
        public string Card_Holder_Name { get; set; }
    }
}
