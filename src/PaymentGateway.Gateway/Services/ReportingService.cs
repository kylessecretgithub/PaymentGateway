using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Models.Entities;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.Services
{
    public class ReportingService
    {
        private readonly PaymentsRepository paymentsRepository;
        private readonly AesEncryption aesEncryption;

        public ReportingService(PaymentsRepository paymentsRepository, AesEncryption aesEncryption)
        {
            this.aesEncryption = aesEncryption;
            this.paymentsRepository = paymentsRepository;
        }

        public async Task<int?> AddPaymentAsync(PaymentRequest paymentRequest, BankResponse bankResponse)
        {
            byte[] encryptedCard = aesEncryption.EncryptString(paymentRequest.CardNumber.ToString());
            
            EntityEntry<PaymentEntity> paymentEntity  = await paymentsRepository.AddPaymentAsync(paymentRequest, bankResponse, encryptedCard);                        
            if (await paymentsRepository.SaveChangesAsync())
            {
                return paymentEntity.Entity.PaymentId;
            }            
            return null;
        }

        public async Task<Payment> GetPaymentAsync(int paymentId)
        {
            Payment payment = await paymentsRepository.GetPaymentAsync(paymentId);
            if (payment == null)
            {
                return null;
            }
            payment.CardNumber = aesEncryption.DecryptToString(payment.EncryptedCardNumber);
            payment.MaskCardNumber();
            return payment;            
        }
    }
}
