using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaymentGateway.Gateway.IntegrationTests.Utilities.Builders;
using PaymentGateway.Gateway.IntegrationTests.Utilities.Fakes;
using PaymentGateway.Gateway.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.IntegrationTests.v1
{
    internal class PaymentControllerIntegrationTests
    { 
        protected HttpClient testingClient;
        protected Dictionary<string, HttpResponseMessage> stubbedResponses;

        [SetUp]
        public void SetUp()
        {
            var bankResponseMessage =  new HttpResponseMessageBuilder().WithJsonContent(new BankResponseBuilder().BuildJson()).Build();
            stubbedResponses = new Dictionary<string, HttpResponseMessage>
            {
                {"/api/ProcessPayment", bankResponseMessage }
            };
            var httpClient = new HttpClientBuilder().WithMessageHandler(new StubHttpMessageHandler(stubbedResponses)).Build();

            var webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(
                builder => builder.ConfigureTestServices(
                    services => 
                    {
                        services.AddScoped(s => new BankFacade(httpClient));
                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    }
                ));

            testingClient = webApplicationFactory.CreateClient();
            testingClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        }

        [TestFixture]
        internal class Post_ProcessPayment_ModelValidation : PaymentControllerIntegrationTests
        {            
            [Test]
            public async Task CardNumber_is_required()
            {
                var jsonRequest = new PaymentRequestBuilder().WithExpiryYear(null).BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            [Test]
            public async Task CVV_is_required()
            {
                var jsonRequest = new PaymentRequestBuilder().WithCVV(null).BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            [Test]
            public async Task ExpiryYear_is_required()
            {
                var jsonRequest = new PaymentRequestBuilder().WithExpiryYear(null).BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            [Test]
            public async Task ExpiryMonth_is_required()
            {
                var jsonRequest = new PaymentRequestBuilder().WithExpiryMonth(null).BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            [Test]
            public async Task Amount_is_required()
            {
                var jsonRequest = new PaymentRequestBuilder().WithAmount(null).BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            [Test]
            public async Task CurrencyISOCode_is_required()
            {
                var jsonRequest = new PaymentRequestBuilder().WithCurrencyISOCode(null).BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            [Test]
            public async Task MerchantId_is_required()
            {
                var jsonRequest = new PaymentRequestBuilder().WithMerchantId(null).BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }

        [TestFixture]
        internal class Post_ProcessPayment_TestProcessesToBankAndSaves : PaymentControllerIntegrationTests
        {           
            private HttpResponseMessage res;

            [SetUp]
            public async Task Setup()
            {
                var jsonRequest = new PaymentRequestBuilder().BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");

                res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);
            }

            [Test]
            public void Returns_ok_response()
            {
                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [TestFixture]
        internal class Post_ProcessPayment_TestFailsToProcessToBankAndSaves : PaymentControllerIntegrationTests
        {
            private HttpResponseMessage res;

            [SetUp]
            public async Task Setup()
            {
                var bankResponseMessage = new HttpResponseMessageBuilder().WithHttpStatusCode(HttpStatusCode.BadRequest).Build();
                stubbedResponses["/api/ProcessPayment"] = bankResponseMessage;
                var jsonRequest = new PaymentRequestBuilder().BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");

                res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);
            }

            [Test]
            public void Returns_bad_request()
            {
                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }
    }
}
