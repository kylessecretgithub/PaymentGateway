using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Models.Entities;
using Serilog;
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
                Log.Information($"Sucessfully saved payment request to database");
                return paymentEntity.Entity.PaymentId;
            }
            Log.Error($"Error saving payment request to database");
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
