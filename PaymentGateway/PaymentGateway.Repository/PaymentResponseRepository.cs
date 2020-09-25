using PaymentGateway.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PaymentGateway.Repository
{
    
    public class PaymentResponseRepository : BaseRepository<AppDbContext>, IPaymentResponseRepository
    {
        private DbSet<PaymentResponse> _paymentResponses;

        public PaymentResponseRepository(AppDbContext dataContext) : base(dataContext)
        {
            this._paymentResponses = dataContext.PaymentResponses;
        }

        public async Task<PaymentResponse> CreateAsync(PaymentResponse paymentResponse)
        {
            await _paymentResponses.AddAsync(paymentResponse);
            await SaveChangesAsync();
            return paymentResponse;
        }

        public async Task<PaymentResponse> GetByAcquirerRefrenceIdAsync(Guid acquirer_reference_id)
        {
            return await _paymentResponses.Where(x => x.Acquirer_Reference_Id == acquirer_reference_id).FirstOrDefaultAsync();
        }

        public async Task<PaymentResponse> GetByTransactionRefrenceIdAsync(Guid transaction_reference_id)
        {
            return await _paymentResponses.Where(x => x.Transaction_Reference_Id == transaction_reference_id).FirstOrDefaultAsync();
        }
    }
}
