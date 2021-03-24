using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaymentGateway.Gateway.IntegrationTests.Utilities.Builders;
using PaymentGateway.Gateway.IntegrationTests.Utilities.Fakes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.IntegrationTests.v1
{
    public class AuthorisationIntegrationTests
    {
        protected HttpClient testingClient;
        private WebApplicationFactory<Startup> webApplicationFactory;

        [SetUp]
        public void BaseSetUp()
        {
            webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(
                builder => builder.ConfigureTestServices(
                    services => services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", options => { })));
            testingClient = webApplicationFactory.CreateClient();
        }

        [TestFixture]
        internal class Get_GetPayment_Authorisation : AuthorisationIntegrationTests
        {
            [Test]
            public async Task Forbidden_returned_when_not_authorised()
            {
                var res = await testingClient.GetAsync("api/v1/Reporting/GetPayment/");

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            }
        }

        [TestFixture]
        internal class Post_ProcessPayment_Authorisation : AuthorisationIntegrationTests
        {
            [Test]
            public async Task Unauthorised_returned_when_not_authorised()
            {
                var jsonRequest = new PaymentRequestBuilder().BuildJson();
                var httpContent = new StringContent(jsonRequest, Encoding.Unicode, "application/json");
                var res = await testingClient.PostAsync("api/v1/Payment/ProcessPayment", httpContent);

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            }
        }
    }
}
