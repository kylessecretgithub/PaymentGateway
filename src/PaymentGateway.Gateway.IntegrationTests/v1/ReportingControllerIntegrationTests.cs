using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.IntegrationTests.Utilities.Builders;
using PaymentGateway.Gateway.IntegrationTests.Utilities.Fakes;
using PaymentGateway.Gateway.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.IntegrationTests.v1
{
    internal class ReportingControllerIntegrationTests
    {
        protected HttpClient testingClient;
        private WebApplicationFactory<Startup> webApplicationFactory;

        [SetUp]
        public async Task BaseSetUp()
        {
            var bankResponseMessage = new HttpResponseMessageBuilder().WithJsonContent(new BankResponseBuilder().BuildJson()).Build();
            var stubbedResponses = new Dictionary<string, HttpResponseMessage>
            {
                {"/api/ProcessPayment", bankResponseMessage }
            };
            var httpClient = new HttpClientBuilder().WithMessageHandler(new StubHttpMessageHandler(stubbedResponses)).Build();

            webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(
                builder => builder.ConfigureTestServices(services =>
                   {
                       services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                       services.AddScoped(s => new BankFacade(httpClient));
                   }
                ));
            using IServiceScope scope = webApplicationFactory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<PaymentGatewayContext>();
            await dbContext.Database.EnsureCreatedAsync();
            dbContext.Add(new PaymentEntityBuilder().Build());
            await dbContext.SaveChangesAsync();
            testingClient = webApplicationFactory.CreateClient();
        }

        [TearDown]
        public async Task BaseTearDown()
        {
            using IServiceScope scope = webApplicationFactory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<PaymentGatewayContext>();
            await dbContext.Database.EnsureDeletedAsync();
        }

        [TestFixture]
        internal class Get_GetPayment_ModelValidation : ReportingControllerIntegrationTests
        {
            [Test]
            public async Task PaymentId_is_requiredAsync()
            {
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment/");

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }

        [TestFixture]
        internal class Get_GetPayment_PaymentRetrievedFromDatabase : ReportingControllerIntegrationTests
        {
            [Test]
            public async Task Ok_response_returned()
            {
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment?paymentId=1");

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }

            [Test]
            public async Task EncryptedCardNumber_is_not_returned()
            {
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment?paymentId=1");
                var content = await res.Content.ReadAsStringAsync();

                Assert.That(content.Contains("encryptedCardNumber"), Is.False);
            }

            [Test]
            public async Task CardNumber_is_only_returned_masked()
            {
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment?paymentId=1");
                var content = await res.Content.ReadAsStringAsync();

                Assert.Multiple(() =>
                {
                   Assert.That(content.Contains("\"maskedCardNumber\":\"123\""), Is.True, "Response content does not contain masked card number as expected");
                   Assert.That(content.Contains("\"cardNumber\":"), Is.False, "Card number property found in response content");
                });

            }
        }

        [TestFixture]
        internal class Get_GetPayment_PaymentNotFoundInDatabase : ReportingControllerIntegrationTests
        {
            [Test]
            public async Task NotFound_status_returned()
            {
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment?paymentId=2");

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }
    }
}
