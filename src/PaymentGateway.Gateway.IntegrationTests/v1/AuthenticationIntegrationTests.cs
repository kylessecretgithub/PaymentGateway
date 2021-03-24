using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using PaymentGateway.Gateway.IntegrationTests.Utilities.Builders;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace PaymentGateway.Gateway.IntegrationTests.v1
{
    public class AuthenticationIntegrationTests
    {
        protected HttpClient testingClient;
        private WebApplicationFactory<Startup> webApplicationFactory;

        [SetUp]
        public void BaseSetUp()
        {
            webApplicationFactory = new WebApplicationFactory<Startup>();
            testingClient = webApplicationFactory.CreateClient();
        }

        [TestFixture]
        internal class Get_GetPayment_Authentication : AuthenticationIntegrationTests
        {
            [Test]
            public async Task Unauthorised_returned_when_not_authenticated()
            {
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment/");

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
        }

        [TestFixture]
        internal class Post_ProcessPayment_Authentication : AuthenticationIntegrationTests
        {
            [Test]
            public async Task Unauthorised_returned_when_not_authenticated()
            {
                var jsonRequest = new PaymentRequestBuilder().BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
        }
    }
}
