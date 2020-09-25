using AutoMapper;
using PaymentGateway.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.DTOs
{
    public class CardRequestDTO
    {
        /// <summary>
        /// Provide the Id of the previously used card. If this is provided, only CVV is needed
        /// </summary>
        public Guid? Id { get; set; }
        public string Number { get; set; }
        public string Expiry_Month { get; set; }
        public string Expiry_Year { get; set; }
        public string Cvv { get; set; }
        public string Card_Holder_Name { get; set; }
    }

    public class CardRequestDTOProfile : Profile
    {
        public CardRequestDTOProfile()
        {
            CreateMap<CardRequestDTO, Card>()
                .ForMember(dest => dest.Card_Holder_Name, s => s.MapFrom(src => src.Card_Holder_Name))
                .ForMember(dest => dest.Cvv, s => s.MapFrom(src => src.Cvv))
                .ForMember(dest => dest.Expiry_Month, s => s.MapFrom(src => src.Expiry_Month))
                .ForMember(dest => dest.Expiry_Year, s => s.MapFrom(src => src.Expiry_Year))
                .ForMember(dest => dest.Number, s => s.MapFrom(src => src.Number))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}
