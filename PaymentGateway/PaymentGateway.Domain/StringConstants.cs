using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain
{
    public static class StringConstants
    {
        public const string MERCHANT_SECRET_KEY_MISMATCH_MESSAGE = "Merchant_Id doesn't belong to the provided Authorization secret_key";
        public const string UNEXPECTED_ERROR_MESSAGE = "An unexpected error occurred";
    }
}
