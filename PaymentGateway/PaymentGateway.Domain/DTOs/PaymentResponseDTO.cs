using AutoMapper;
using Newtonsoft.Json.Converters;
using PaymentGateway.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PaymentGateway.Domain.DTOs
{
    public class PaymentResponseDTO
    {
        
        public Guid Transaction_Reference_Id { get; set; }

        public Guid Acquirer_Reference_Id { get; set; }

        public string Currency { get; set; }
        public double Amount { get; set; }

        public CardResponseDTO Card_Info { get; set; }
       
        public StatusCode Status { get; set; }

        public DateTime Processed_On { get; set; }

        public IEnumerable<String> Errors { get; set; }
    }

    public class PaymentResponseDTOProfile : Profile
    {
        public PaymentResponseDTOProfile()
        {
            CreateMap<PaymentRequestDTO, PaymentResponseDTO>()
                .ForMember(dest => dest.Currency, s => s.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Amount, s => s.MapFrom(src => src.Amount))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PaymentResponse, PaymentResponseDTO>()
                .ForMember(dest => dest.Currency, s => s.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Amount, s => s.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Transaction_Reference_Id, s => s.MapFrom(src => src.Transaction_Reference_Id))
                .ForMember(dest => dest.Acquirer_Reference_Id, s => s.MapFrom(src => src.Acquirer_Reference_Id))
                .ForMember(dest => dest.Processed_On, s => s.MapFrom(src => src.Processed_On))
                .ForMember(dest => dest.Status, s => s.MapFrom(src => Enum.Parse(typeof(StatusCode),src.Status.ToString())))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PaymentResponseDTO, PaymentResponse>()
                .ForMember(dest => dest.Currency, s => s.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Amount, s => s.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Transaction_Reference_Id, s => s.MapFrom(src => src.Transaction_Reference_Id))
                .ForMember(dest => dest.Acquirer_Reference_Id, s => s.MapFrom(src => src.Acquirer_Reference_Id))
                .ForMember(dest => dest.Processed_On, s => s.MapFrom(src => src.Processed_On))
                .ForMember(dest => dest.Status, s => s.MapFrom(src => src.Status))
                .ForMember(dest => dest.Card_id, s => s.MapFrom(src => src.Card_Info.Id))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}
