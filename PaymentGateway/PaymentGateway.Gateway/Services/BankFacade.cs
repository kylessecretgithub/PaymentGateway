using PaymentGateway.Gateway.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.Services
{
    public class BankFacade
    {
        private readonly HttpClient httpClient;

        public BankFacade(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<BankResponse> ProcessPayment(PaymentRequest paymentRequest)
        {
            throw new NotImplementedException();
        }
    }
}
