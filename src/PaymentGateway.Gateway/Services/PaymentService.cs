using PaymentGateway.Gateway.Models;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.Services
{
    public class PaymentService
    {
        private readonly ReportingService reportingService;
        private readonly BankFacade bank;

        public PaymentService(ReportingService reportingService, BankFacade bank)
        {
            this.reportingService = reportingService;
            this.bank = bank;
        }

        public async Task<PaymentProcessedResponse> SendPaymentToBankAndSaveAsync(PaymentRequest paymentRequest)
        {
            var bankResponse = await bank.ProcessPaymentAsync(paymentRequest);
            var paymentId =  await reportingService.AddPaymentAsync(paymentRequest, bankResponse);
            if (paymentId == null && bankResponse.Status == "Processed")
            {
                return new PaymentProcessedResponse(paymentId, "Failed to save processed payment");
            }
            return new PaymentProcessedResponse(paymentId, bankResponse.Status);
        }
    }
}
