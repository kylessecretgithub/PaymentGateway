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
        public ReportingService(PaymentsRepository paymentsRepository)
        {
            this.paymentsRepository = paymentsRepository;
        }

        public async Task<int?> AddPaymentAsync(PaymentRequest paymentRequest, BankResponse bankResponse)
        {
            EntityEntry<PaymentEntity> paymentEntity  = await paymentsRepository.AddPaymentAsync(paymentRequest, bankResponse);                        
            if (await paymentsRepository.SaveChangesAsync())
            {
                return paymentEntity.Entity.PaymentId;
            }
            return null;
        }
    }
}
