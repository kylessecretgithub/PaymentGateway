using IdentityModel.Client;
using Newtonsoft.Json;
using Runner;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        public static string PaymentIdToFetch = "1";
        public static string PaymentAmountToPost = "100";


        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            Console.WriteLine("Fetching discovery doc for auth");
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:44392");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            Console.WriteLine("Fetching token for auth");
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
               
                ClientId = "Merchant",
                ClientSecret = "secret",
                Scope = "ProcessPayment GetPayment"
            });
            client.SetBearerToken(tokenResponse.AccessToken);


            var postResponse = await client.PostAsync("https://localhost:44306/api/v1/Payment/ProcessPayment", new StringContent(PaymentRequest.GetNewJson(PaymentAmountToPost), Encoding.UTF8, "application/json"));
            var postContent = await postResponse.Content.ReadAsStringAsync();

            Console.WriteLine("Response from post:");
            Console.WriteLine(postContent);

            var getResponse = await client.GetAsync("https://localhost:44306/api/v1/Reporting/GetPayment?paymentId="+ PaymentIdToFetch);
            var getContent = await getResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Response from get:");
            Console.WriteLine(getContent);
            Console.ReadLine();
        }
    }
}
