using PaymentGateway.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;

namespace PaymentGateway.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Merchant> Merchants { get; set; }
        public virtual DbSet<PaymentResponse> PaymentResponses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source = PaymentGateway.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Seed Data For Currencies
            modelBuilder.Entity<Currency>().HasData(
               new { Code = "USD", Name = "US Dollar" },
               new { Code = "AUD", Name = "Australian Dollar" },
               new { Code = "CAD", Name = "Canadian Dollar" },
               new { Code = "INR", Name = "Indian Rupee" },
               new { Code = "EUR", Name = "Euro" }
               );           
        }
    }
}