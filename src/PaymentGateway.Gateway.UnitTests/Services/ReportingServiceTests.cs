using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Factories;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Services;
using PaymentGateway.Gateway.UnitTests.Utilities.Builders;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.UnitTests.Services
{
    internal class ReportingServiceTests
    {
        protected PaymentGatewayContext context;
        protected ReportingService reportingService;
        protected DbContextOptionsBuilder<PaymentGatewayContext> optionsBuilder;
        protected Guid loggingContextGuid;
        private SqliteConnection connection;

        [SetUp]
        public void BaseSetUp()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
            loggingContextGuid = TestCorrelator.CreateContext().Guid;
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            optionsBuilder = new DbContextOptionsBuilder<PaymentGatewayContext>();
            optionsBuilder.UseSqlite(connection).EnableSensitiveDataLogging();
            using (var paymentGatewayContext = new PaymentGatewayContext(optionsBuilder.Options))
            {
                paymentGatewayContext.Database.EnsureCreated();
            }
            context = new PaymentGatewayContext(optionsBuilder.Options);
            var paymentsRepository = new PaymentsRepository(context);
            var aesKey = new AesKey("hunter2");
            reportingService = new ReportingService(paymentsRepository, new AesEncryption(new RandomNumberGeneratorProxyFactory(), aesKey));
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
        internal class AddPaymentAsync_PaymentIsSuccessfullyAdded : ReportingServiceTests
        {
            private int? paymentId;

            [SetUp]
            public async Task SetUp()
            {
                paymentId = await reportingService.AddPaymentAsync(new PaymentRequestBuilder().Build(), new BankResponseBuilder().Build());
            }

            [Test]
            public void Returns_paymentId()
            {
                Assert.That(paymentId, Is.EqualTo(1));
            }

            [Test]
            public void Logs_entity_has_been_saved()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Sucessfully saved payment request to database")), Is.EqualTo(1));
            }
        }

        [TestFixture]
        internal class AddPaymentAsync_ErrorSavingToDb : ReportingServiceTests
        {
            private int? paymentId;

            [SetUp]
            public async Task SetUp()
            {
                var unsaveablePaymentRequest = new PaymentRequestBuilder().WithCurrencyISOCode(null).Build();
                paymentId = await reportingService.AddPaymentAsync(unsaveablePaymentRequest, new BankResponseBuilder().Build());
            }

            [Test]
            public void Returns_null()
            {
                Assert.That(paymentId, Is.Null);
            }

            [Test]
            public void Logs_entity_has_been_saved()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Error saving payment request to database")), Is.EqualTo(1));
            }
        }

        [TestFixture]
        internal class GetPaymentAsync_PaymentInDatabase : ReportingServiceTests
        {
            private Payment payment;

            [SetUp]
            public async Task SetUp()
            {
                await context.AddAsync(new PaymentEntityBuilder().Build());
                await context.SaveChangesAsync();
                payment = await reportingService.GetPaymentAsync(1);
            }

            [Test]
            public void Payment_returned()
            {
                Assert.That(payment, Is.Not.Null);
            }

            [Test]
            public void CardNumber_is_masked()
            {
                Assert.That(payment.MaskedCardNumber, Is.EqualTo("123"));
            }

            [Test]
            public void Unencrypted_cardNumber_does_not_leave_service_layer()
            {
                Assert.That(payment.CardNumber, Is.Null);
            }
        }


        [TestFixture]
        internal class GetPaymentAsync_NoMatchingPaymentInDatabase : ReportingServiceTests
        {
            private Payment payment;

            [SetUp]
            public async Task SetUp()
            {
                payment = await reportingService.GetPaymentAsync(1);
            }

            [Test]
            public void Null_returned()
            {
                Assert.That(payment, Is.Null);
            }
        }
    }
}
