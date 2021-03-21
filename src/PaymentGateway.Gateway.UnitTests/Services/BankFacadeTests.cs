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
    internal class BankFacadeTests
    {
        protected BankFacade bankFacade;
        protected Dictionary<string, HttpResponseMessage> stubbedResponses;

        [SetUp]
        internal void BaseSetUp()
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
        private class ProcessPaymentAsync_RequestIsAcceptedByBank : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            internal async Task SetUp()
            {
                response = await bankFacade.ProcessPaymentAsync(new PaymentRequestBuilder().Build());
            }

            [Test]
            internal void BankResponse_has_properties_populated()
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
        private class ProcessPaymentAsync_ErrorRequestingBankWithErrorContent : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            internal async Task SetUp()
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
                response = await bankFacade.ProcessPaymentAsync(new PaymentRequestBuilder().Build());
            }

            [Test]
            internal void BankResponse_has_properties_populated()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.DetailedMessage, Is.EqualTo("unable to find merchant"), "Detailed Message not populated with expected value");
                    Assert.That(response.Status, Is.EqualTo("merchant not recognised"), "Status not populated with expected value");
                });
            }
        }


        [TestFixture]
        private class ProcessPaymentAsync_OkRequestWithInvalidJsonContentReturned : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            internal async Task SetUp()
            {
                var badResponse = new HttpResponseMessageBuilder()
                    .WithHttpStatusCode(HttpStatusCode.OK)
                    .WithJsonContent("My name is Jason, not JSON!")
                    .Build();
                stubbedResponses["/api/ProcessPayment"] = badResponse;
                response = await bankFacade.ProcessPaymentAsync(new PaymentRequestBuilder().Build());
            }

            [Test]
            internal void BankResponse_has_only_failed_status_populated()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.Status, Is.EqualTo("Failed"), "Status not populated with expected value");
                    Assert.That(response.DetailedMessage, Is.Null, "Detailed Message not populated with expected value");
                    Assert.That(response.PaymentId, Is.Null, "Detailed Message not populated with expected value");
                });
            }
        }


        [TestFixture]
        private class ProcessPaymentAsync_ErrorRequestWithInvalidJsonContentReturned : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            internal async Task SetUp()
            {
                var badResponse = new HttpResponseMessageBuilder()
                    .WithHttpStatusCode(HttpStatusCode.BadRequest)
                    .WithJsonContent("My name is Jason, not JSON!")
                    .Build();
                stubbedResponses["/api/ProcessPayment"] = badResponse;
                response = await bankFacade.ProcessPaymentAsync(new PaymentRequestBuilder().Build());
            }

            [Test]
            internal void BankResponse_has_only_failed_status_populated()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.Status, Is.EqualTo("Failed"), "Status not populated with expected value");
                    Assert.That(response.DetailedMessage, Is.Null, "Detailed Message not populated with expected value");
                    Assert.That(response.PaymentId, Is.Null, "Detailed Message not populated with expected value");
                });
            }
        }

        [TestFixture]
        private class ProcessPaymentAsync_ErrorWithRequest : BankFacadeTests
        {
            private BankResponse response;

            [SetUp]
            internal async Task SetUp()
            {
                var badResponse = new HttpResponseMessageBuilder()
                    .WithHttpStatusCode(HttpStatusCode.BadRequest)
                    .Build();
                stubbedResponses["/api/ProcessPayment"] = badResponse;
                response = await bankFacade.ProcessPaymentAsync(new PaymentRequestBuilder().Build());
            }

            [Test]
            internal void BankResponse_has_only_failed_status_populated()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.Status, Is.EqualTo("Failed"), "Status not populated with expected value");
                    Assert.That(response.DetailedMessage, Is.Null, "Detailed Message not populated with expected value");
                    Assert.That(response.PaymentId, Is.Null, "Detailed Message not populated with expected value");
                });
            }
        }
    }
}
