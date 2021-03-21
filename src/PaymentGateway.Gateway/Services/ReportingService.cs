using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Models;
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

        public async Task<bool> AddPaymentAsync(PaymentRequest paymentRequest, BankResponse bankResponse)
        {
            await paymentsRepository.AddPaymentAsync(paymentRequest, bankResponse);
            return await paymentsRepository.SaveChangesAsync();
        }
    }
}
