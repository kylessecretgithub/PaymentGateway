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
        internal async Task BaseSetUp()
        {
            var bankResponseMessage = new HttpResponseMessageBuilder().WithJsonContent(new BankResponseBuilder().BuildJson()).Build();
            var stubbedResponses = new Dictionary<string, HttpResponseMessage>
            {
                {"/api/ProcessPayment", bankResponseMessage }
            };
            var httpClient = new HttpClientBuilder().WithMessageHandler(new StubHttpMessageHandler(stubbedResponses)).Build();

            webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(
                builder => builder.ConfigureTestServices(
                    services => services.AddScoped(s => new BankFacade(httpClient))                    
                ));
            using IServiceScope scope = webApplicationFactory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<PaymentGatewayContext>();
            await dbContext.Database.EnsureCreatedAsync();
            dbContext.Add(new PaymentEntityBuilder().Build());
            await dbContext.SaveChangesAsync();
            testingClient = webApplicationFactory.CreateClient();
        }

        [TearDown]
        internal async Task BaseTearDown()
        {
            using IServiceScope scope = webApplicationFactory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<PaymentGatewayContext>();
            await dbContext.Database.EnsureDeletedAsync();
        }

        internal class Get_GetPayment_ModelValidation : ReportingControllerIntegrationTests
        {
            [Test]
            public async Task PaymentId_is_requiredAsync()
            {
                var jsonRequest = new PaymentRequestBuilder().WithCVV(null).BuildJson();
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment/");

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }

        internal class Get_GetPayment_PaymentRetrievedFromDatabase : ReportingControllerIntegrationTests
        {
            [Test]
            public async Task Ok_status_returned()
            {
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment?paymentId=1");

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
        }

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
