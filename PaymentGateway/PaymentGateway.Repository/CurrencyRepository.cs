using PaymentGateway.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Repository
{
    public class CurrencyRepository : BaseRepository<AppDbContext>, ICurrencyRepository
    {
        private DbSet<Currency> _currencies;
        public CurrencyRepository(AppDbContext dataContext) : base(dataContext)
        {
            this._currencies = dataContext.Currencies;
        }

        public IQueryable<Currency> QueryAll()
        {
            return _currencies.AsQueryable();
        }
    }
}
