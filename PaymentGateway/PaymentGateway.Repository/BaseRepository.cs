using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.Repository
{
    public interface IBaseRepository
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }

    public abstract class BaseRepository<TContext> : IBaseRepository where TContext : DbContext
    {
        protected readonly TContext DataContext;
        public BaseRepository(TContext dataContext)
        {
            this.DataContext = dataContext;
        }

        public int SaveChanges()
        {
            return this.DataContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.DataContext.SaveChangesAsync();
        }
    }
}
