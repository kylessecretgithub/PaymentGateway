using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.UnitTests.Services
{
    internal class PaymentServiceTests
    {
        private DbContextOptionsBuilder<PaymentGatewayContext> optionsBuilder;
        private PaymentGatewayContext context;
        private SqliteConnection connection;
        private BankFacade bankFacade;
        protected Dictionary<string, HttpResponseMessage> stubbedResponses;
        protected PaymentService paymentService;
        protected Guid loggingContextGuid;

        [SetUp]
        public void BaseSetUp()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
            loggingContextGuid = TestCorrelator.CreateContext().Guid;
            SetUpDatabse();
            SetUpBankFacade();
            var paymentsRepository = new PaymentsRepository(context);
            var aesKey = new AesKey("Zq3t6w9z$C&F)J@N");
            var reportingService = new ReportingService(paymentsRepository, new AesEncryption(new RandomNumberGeneratorProxyFactory(), aesKey));
            paymentService = new PaymentService(reportingService, bankFacade);
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

        public void SetUpDatabse()
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

        [TearDown]
        public async Task BaseTearDown()
        {
            using (var paymentGatewayContext = new PaymentGatewayContext(optionsBuilder.Options))
            {
                await context.Database.EnsureDeletedAsync();
            }
            connection.Close();
        }

        [TestFixture]
        internal class SendPaymentToBankAndSaveAsync_PaymentProcessesAndSavesToDatabase : PaymentServiceTests
        {
            private PaymentProcessedResponse response;

            [SetUp]
            public async Task SetUp()
            {
                response = await paymentService.SendPaymentToBankAndSaveAsync(new PaymentRequestBuilder().Build());
            }

            [Test]
            public void PaymentResponse_has_partial_failed_properties()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.PaymentId, Is.EqualTo(1), "PaymentId is not expected value");
                    Assert.That(response.Status, Is.EqualTo("Processed"), "Status is not expected value");
                });
            }

            [Test]
            public void Logs_status_returned_from_bank()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Status returned from processing payment to bank: Processed")), Is.EqualTo(1));
            }
        }

        [TestFixture]
        internal class SendPaymentToBankAndSaveAsync_PaymentProcessedToBankAndFailsToSaveToDatabase : PaymentServiceTests
        {
            private PaymentProcessedResponse response;

            [SetUp]
            public async Task SetUp()
            {
                var unsaveablePaymentRequest = new PaymentRequestBuilder().WithCurrencyISOCode(null).Build();
                response = await paymentService.SendPaymentToBankAndSaveAsync(unsaveablePaymentRequest);
            }

            [Test]
            public void PaymentResponse_has_partial_failed_properties()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.PaymentId, Is.Null, "PaymentId is not null");
                    Assert.That(response.Status, Is.EqualTo("Failed to save processed payment"), "Status is not expected value");
                });
            }

            [Test]
            public void Logs_status_returned_from_bank()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Status returned from processing payment to bank: Processed")), Is.EqualTo(1));
            }
        }
    }
}
