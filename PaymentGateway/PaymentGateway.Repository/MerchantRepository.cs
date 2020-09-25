using PaymentGateway.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace PaymentGateway.Repository
{
    public class MerchantRepository : BaseRepository<AppDbContext>, IMerchantRepository
    {
        private DbSet<Merchant> _merchants;
        private readonly IDataProtector _protector;
        public MerchantRepository(AppDbContext dataContext, IDataProtectionProvider protectionProvider) : base(dataContext)
        {
            this._merchants = dataContext.Merchants;
            _protector = protectionProvider.CreateProtector("merchant_encryption_key_6187009c-c39a-49bc-8343-ee42efcd1e5f");
        }

        public IEnumerable<Merchant> GetAll()
        {
            var allMerchants = _merchants.AsNoTracking().AsEnumerable();
            var allMerchantsWithUnprotectedKey = allMerchants.Select(x =>
            {
                x.Secret_Key = _protector.Unprotect(x.Secret_Key);
                return x;
            });
            return allMerchantsWithUnprotectedKey;
        }

        public async Task<Merchant> AddAsync(Merchant merchant)
        {
            merchant.Id = Guid.NewGuid();
            string generated_key = String.Concat("key_", GenerateRandomCryptographicKey(32));
            merchant.Secret_Key = _protector.Protect(generated_key);
            await _merchants.AddAsync(merchant);
            await SaveChangesAsync();
            merchant.Secret_Key = generated_key;
            return merchant;
        }

        public async Task<Guid> GetMerchantIdBySecretKey(string secret_key)
        {
            var merchants = await _merchants.ToListAsync();
            foreach (var merchant in merchants)
            {
                if(String.Equals(_protector.Unprotect(merchant.Secret_Key),secret_key))
                {
                    return merchant.Id;
                }
            }
            return Guid.Empty;
        }

        private string GenerateRandomCryptographicKey(int keyLength)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
