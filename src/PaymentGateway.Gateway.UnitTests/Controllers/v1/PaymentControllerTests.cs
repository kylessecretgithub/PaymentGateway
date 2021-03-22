using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PaymentGateway.Gateway.Controllers.v1;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Factories;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Services;
using PaymentGateway.Gateway.UnitTests.Utilities.Builders;
using PaymentGateway.Gateway.UnitTests.Utilities.Fakes;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.UnitTests.Controllers.v1
{
    internal class PaymentControllerTests
    {
        private DbContextOptionsBuilder<PaymentGatewayContext> optionsBuilder;
        private PaymentGatewayContext context;
        private SqliteConnection connection;
        private BankFacade bankFacade;
        protected Dictionary<string, HttpResponseMessage> stubbedResponses;
        protected PaymentController paymentController;
        protected Guid loggingContextGuid;

        [SetUp]
        public void BaseSetUp()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
            loggingContextGuid = TestCorrelator.CreateContext().Guid;

            SetUpDatabase();
            SetUpBankFacade();
            var paymentsRepository = new PaymentsRepository(context);
            var aesKey = new AesKey("hunter2");
            var reportingService = new ReportingService(paymentsRepository, new AesEncryption(new RandomNumberGeneratorProxyFactory(), aesKey));
            var paymentService = new PaymentService(reportingService, bankFacade);
            paymentController = new PaymentController(paymentService);
        }

        private void SetUpBankFacade()
        {
            var okBankHttpResponse = new HttpResponseMessageBuilder().WithJsonContent(new BankResponseBuilder().BuildJson()).Build();
            stubbedResponses = new Dictionary<string, HttpResponseMessage>
            {
                {"/api/ProcessPayment", okBankHttpResponse }
            };
            var httpClient = new HttpClientBuilder().WithMessageHandler(new StubHttpMessageHandler(stubbedResponses)).Build();
            bankFacade = new BankFacade(httpClient);
        }

        public void SetUpDatabase()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            optionsBuilder = new DbContextOptionsBuilder<PaymentGatewayContext>();
            optionsBuilder.UseSqlite(connection).EnableSensitiveDataLogging();
            using (var paymentGatewayContext = new PaymentGatewayContext(optionsBuilder.Options))
            {
                paymentGatewayContext.Database.EnsureCreated();
            }
            context = new PaymentGatewayContext(optionsBuilder.Options);
        }

        internal class ProcessPaymentAsync_PaymentProcesses : PaymentControllerTests
        {
            private IActionResult response;

            [SetUp]
            public async Task SetUp()
            {
                response = await paymentController.ProcessPaymentAsync(new PaymentRequestBuilder().Build());
            }

            public void Type_is_okObjectResult()
            {
                Assert.That(response, Is.InstanceOf<OkObjectResult>());
            }

            [Test]
            public void Returns_processing_details_in_response()
            {                
                var okObject = response as OkObjectResult;
                var paymentResponse = okObject.Value as PaymentProcessedResponse;
                Assert.Multiple(() =>
                {
                    Assert.That(paymentResponse.PaymentId, Is.EqualTo(1), "PaymentId is not expected value");
                    Assert.That(paymentResponse.Status, Is.EqualTo("Processed"), "Status is not expected value");
                });
            }

            [Test]
            public void Logs_incoming_request_message()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Incoming payment request")), Is.EqualTo(1));
            }

            [Test]
            public void Logs_sucessful_request_message()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Sucessfully processed payment to bank")), Is.EqualTo(1));
            }
        }
        internal class ProcessPaymentAsync_PaymentProcessesButFailsToSaveInDatabase : PaymentControllerTests
        {
            private IActionResult response;

            [SetUp]
            public async Task SetUp()
            {
                response = await paymentController.ProcessPaymentAsync(new PaymentRequestBuilder().WithCurrencyISOCode(null).Build());
            }

            [Test]
            public void Type_is_okObjectResult()
            {
                Assert.That(response, Is.InstanceOf<OkObjectResult>());
            }

            [Test]
            public void Returns_processing_details_in_response()
            {
                var okObject = response as OkObjectResult;
                var paymentResponse = okObject.Value as PaymentProcessedResponse;
                Assert.Multiple(() =>
                {
                    Assert.That(paymentResponse.PaymentId, Is.Null, "PaymentId is not null");
                    Assert.That(paymentResponse.Status, Is.EqualTo("Failed to save processed payment"), "Status is not expected value");
                });
            }

            [Test]
            public void Logs_incoming_request_message()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Incoming payment request")), Is.EqualTo(1));
            }

            [Test]
            public void Logs_sucessful_request_message()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Sucessfully processed payment to bank")), Is.EqualTo(1));
            }
        }

        internal class ProcessPaymentAsync_PaymentFailsToProcessToBank : PaymentControllerTests
        {
            private IActionResult response;

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
                response = await paymentController.ProcessPaymentAsync(new PaymentRequestBuilder().Build());
            }

            [Test]
            public void Type_is_badRequestObjectResult()
            {
                Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            }

            [Test]
            public void Returns_processing_details_in_response()
            {
                var okObject = response as BadRequestObjectResult;
                var paymentResponse = okObject.Value as PaymentProcessedResponse;
                Assert.Multiple(() =>
                {
                    Assert.That(paymentResponse.PaymentId, Is.EqualTo(1), "PaymentId is not expected value");
                    Assert.That(paymentResponse.Status, Is.EqualTo("merchant not recognised"), "Status is not expected value");
                });
            }

            [Test]
            public void Logs_incoming_request_message()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Incoming payment request")), Is.EqualTo(1));
            }

            [Test]
            public void Logs_failed_request_message()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Failed to process payment to bank")), Is.EqualTo(1));
            }
        }
    }
}
