using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Models.Entities;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.DataAccess
{
    public class PaymentsRepository
    {
        private readonly PaymentGatewayContext dbContext;

        public PaymentsRepository(PaymentGatewayContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<EntityEntry<PaymentEntity>> AddPaymentAsync(PaymentRequest paymentRequest, BankResponse bankResponse)
        {
            var payment = new PaymentEntity
            {
                Amount = paymentRequest.Amount,
                CardNumber = paymentRequest.CardNumber,
                CurrencyISOCode = paymentRequest.CurrencyISOCode,
                CVV = paymentRequest.CVV,
                ExpiryMonth = paymentRequest.ExpiryMonth,
                ExpiryYear = paymentRequest.ExpiryYear,
                MerchantId = paymentRequest.MerchantId,
                BankPaymentId = bankResponse.PaymentId,
                Status = bankResponse.Status
            };
            return await dbContext.AddAsync(payment);
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return false;
            }
            return true;
        }
    }
}
