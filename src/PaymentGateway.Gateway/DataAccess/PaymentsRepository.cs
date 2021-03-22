using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Models.Entities;
using System.Linq;
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

        public async Task<EntityEntry<PaymentEntity>> AddPaymentAsync(PaymentRequest paymentRequest, BankResponse bankResponse, byte[] encryptedCard)
        {
            var payment = new PaymentEntity
            {
                Amount = paymentRequest.Amount.Value,
                EncryptedCardNumber = encryptedCard,
                CurrencyISOCode = paymentRequest.CurrencyISOCode,
                CVV = paymentRequest.CVV.Value,
                ExpiryMonth = paymentRequest.ExpiryMonth.Value,
                ExpiryYear = paymentRequest.ExpiryYear.Value,
                MerchantId = paymentRequest.MerchantId.Value,
                BankPaymentId = bankResponse.PaymentId,
                Status = bankResponse.Status
            };
            return await dbContext.AddAsync(payment);
        }

        public async Task<Payment> GetPaymentAsync(int paymentRequestId)
        {            
            return await dbContext.Payments
                .Where(p => p.PaymentId == paymentRequestId)
                .Select(pe => new Payment 
                {
                    Amount = pe.Amount,
                    EncryptedCardNumber = pe.EncryptedCardNumber,
                    CurrencyISOCode = pe.CurrencyISOCode,
                    CVV = pe.CVV,
                    ExpiryMonth = pe.ExpiryMonth,
                    ExpiryYear = pe.ExpiryYear,
                    MerchantId = pe.MerchantId,                
                    BankPaymentId = pe.BankPaymentId,
                    Status = pe.Status
                })
                .SingleOrDefaultAsync();
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
