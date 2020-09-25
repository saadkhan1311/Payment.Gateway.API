using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain.DTOs
{
    public enum StatusCode
    {
        APPROVED = 1000,
        INSUFFICIENT_BALANCE = 2000,
        INVALID_DATA = 3000,
        UNAUTHORIZED = 4000,
        UNKNOWN = 9000,
    }
}
