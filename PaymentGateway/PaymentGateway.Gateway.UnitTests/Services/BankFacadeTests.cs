using NUnit.Framework;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Services;
using PaymentGateway.Gateway.UnitTests.Utilities.Builders;
using PaymentGateway.Gateway.UnitTests.Utilities.Fakes;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.UnitTests.Services
{
    public class BankFacadeTests
    {
        protected BankFacade bankFacade;
        protected Dictionary<string, HttpResponseMessage> stubbedResponses;

        [SetUp]
        public void BaseSetUp()
        {
            var okBankHttpResponse = new HttpResponseMessageBuilder().WithJsonContent(new BankResponseBuilder().BuildJson()).Build();
            stubbedResponses = new Dictionary<string, HttpResponseMessage>
            {
                {"/api/ProcessPayment", okBankHttpResponse }
            };
            var httpClient = new HttpClientBuilder().WithMessageHandler(new StubHttpMessageHandler(stubbedResponses)).Build();
            bankFacade = new BankFacade(httpClient);
        }

        [TestFixture]
        private class SubmitPayment_RequestIsAcceptedByBank : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            public async Task SetUp()
            {
                response = await bankFacade.ProcessPayment(new PaymentRequestBuilder().Build());
            }

            [Test]
            public void BankResponse_has_properties_populated()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.DetailedMessage, Is.EqualTo("Processed payment sucessfully"), "Detailed Message not populated with expected value");
                    Assert.That(response.Status, Is.EqualTo("Processed"), "Status not populated with expected value");
                    Assert.That(response.PaymentId, Is.EqualTo(1), "PaymentId not populated with expected value");
                });
            }
        }

        [TestFixture]
        private class SubmitPayment_ErrorRequestingBankWithErrorContent : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            public async Task SetUp()
            {
                var errorBankResponse = new BankResponseBuilder()
                    .WithDetailedMessage("unable to find merchant")
                    .WithStatus("merchant not recognised")
                    .WithPaymentId(null)
                    .BuildJson();
                var badResponse = new HttpResponseMessageBuilder()
                    .WithHttpStatusCode(HttpStatusCode.NotFound)
                    .WithJsonContent(errorBankResponse)
                    .Build();
                stubbedResponses["/api/ProcessPayment"] = badResponse;
                response = await bankFacade.ProcessPayment(new PaymentRequestBuilder().Build());
            }

            [Test]
            public void BankResponse_has_properties_populated()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.DetailedMessage, Is.EqualTo("unable to find merchant"), "Detailed Message not populated with expected value");
                    Assert.That(response.Status, Is.EqualTo("merchant not recognised"), "Status not populated with expected value");
                });
            }
        }


        [TestFixture]
        private class SubmitPayment_InvalidJsonContentReturned : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            public async Task SetUp()
            {
                var badResponse = new HttpResponseMessageBuilder()
                    .WithHttpStatusCode(HttpStatusCode.OK)
                    .WithJsonContent("My name is Jason, not JSON!")
                    .Build();
                stubbedResponses["/api/ProcessPayment"] = badResponse;
                response = await bankFacade.ProcessPayment(new PaymentRequestBuilder().Build());
            }

            [Test]
            public void Returns_null()
            {
                Assert.That(response, Is.EqualTo(null));
            }
        }

        [TestFixture]
        private class SubmitPayment_ErrorWithRequest : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            public async Task SetUp()
            {
                var badResponse = new HttpResponseMessageBuilder()
                    .WithHttpStatusCode(HttpStatusCode.BadRequest)
                    .Build();
                stubbedResponses["/api/ProcessPayment"] = badResponse;
                response = await bankFacade.ProcessPayment(new PaymentRequestBuilder().Build());
            }

            [Test]
            public void Returns_null()
            {
                Assert.That(response, Is.EqualTo(null));
            }
        }
    }
}
