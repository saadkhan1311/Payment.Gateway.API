using PaymentGateway.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Repository
{
    public interface IPaymentResponseRepository : IBaseRepository
    {
        Task<PaymentResponse> CreateAsync(PaymentResponse paymentResponse);
        Task<PaymentResponse> GetByAcquirerRefrenceIdAsync(Guid acquirer_reference_id);
        Task<PaymentResponse> GetByTransactionRefrenceIdAsync(Guid transaction_reference_id);
    }
}
