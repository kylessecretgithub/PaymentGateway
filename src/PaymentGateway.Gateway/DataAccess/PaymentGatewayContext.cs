using Microsoft.EntityFrameworkCore;
using PaymentGateway.Gateway.Models.Entities;

namespace PaymentGateway.Gateway.DataAccess
{
    public class PaymentGatewayContext : DbContext
    {
        public PaymentGatewayContext(DbContextOptions<PaymentGatewayContext> options)
        : base(options)
        {
        }

        public virtual DbSet<PaymentEntity> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentEntity>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Amount).IsRequired();
                entity.Property(e => e.CardNumber).IsRequired();
                entity.Property(e => e.CurrencyISOCode).IsRequired();
                entity.Property(e => e.CVV).IsRequired();
                entity.Property(e => e.ExpiryMonth).IsRequired();
                entity.Property(e => e.ExpiryYear).IsRequired();
                entity.Property(e => e.Created).IsRequired();
                entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
            });
        }
    }
}