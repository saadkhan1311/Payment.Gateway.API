using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.DTOs
{
    public interface IRequestInfo
    {
        Guid User_Id { get; set; }
        Guid Transaction_Reference { get; set; }
    }

    public class RequestInfo : IRequestInfo
    {
        public Guid User_Id { get; set; }
        public Guid Transaction_Reference { get; set; }
    }
}
