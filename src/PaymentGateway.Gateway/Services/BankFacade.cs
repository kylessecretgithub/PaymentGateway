using Newtonsoft.Json;
using PaymentGateway.Gateway.Models;
using Serilog;
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

        public async Task<BankResponse> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            string processPaymentEndpoint = $"api/ProcessPayment";
            var request = new HttpRequestMessage(HttpMethod.Post, processPaymentEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
            Log.Information("Attempting to send payment request to bank");
            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Log.Information("Successful response returned from bank");
                var content = await response.Content.ReadAsStringAsync();
                return TryDeserialiseBankResponse(content);
            }
            Log.Error("Unsuccessful response returned from bank");
            if (response.Content != null)
            {
                string errorJson = await response.Content.ReadAsStringAsync();
                return TryDeserialiseBankResponse(errorJson);
            }
            return new BankResponse("Failed");
        }

        public BankResponse TryDeserialiseBankResponse(string content)
        {
            BankResponse bankResponse;
            try
            {
                bankResponse = JsonConvert.DeserializeObject<BankResponse>(content);
                return bankResponse;
            }
            catch (Exception e)
            {
                Log.Error($"Error deserialising bank payload. Exception: {e.Message}");
                return new BankResponse("Failed");
            }
        }
    }
}
