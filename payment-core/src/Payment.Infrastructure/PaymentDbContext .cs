using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Data
{
    public class PaymentDbContext : DbContext
    {

        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options)
        {

        }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

    }
}