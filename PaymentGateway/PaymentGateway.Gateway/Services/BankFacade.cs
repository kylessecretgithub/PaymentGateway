using Newtonsoft.Json;
using PaymentGateway.Gateway.Models;
using System;
using System.Net.Http;
using System.Text;
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
            string processPaymentEndpoint = $"api/ProcessPayment";
            var request = new HttpRequestMessage(HttpMethod.Post, processPaymentEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                BankResponse bankResponse;
                try
                {
                    bankResponse = JsonConvert.DeserializeObject<BankResponse>(content);
                    return bankResponse;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                if (response.Content != null)
                {
                    string errorJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BankResponse>(errorJson);
                }
                return null;
            }
        }
    }
}
