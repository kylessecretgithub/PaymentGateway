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
using Serilog;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.UnitTests.Controllers.v1
{
    internal class ReportingControllerTests
    {
        protected PaymentGatewayContext context;
        protected ReportingController reportingController;
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
            var reportingService = new ReportingService(paymentsRepository, new AesEncryption(new RandomNumberGeneratorProxyFactory(), aesKey));
            reportingController = new ReportingController(reportingService);
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


        public class GetPaymentAsync_PaymentIsRetrieved : ReportingControllerTests
        {
            private IActionResult response;

            [SetUp]
            public async Task SetUp()
            {
                await context.AddAsync(new PaymentEntityBuilder().Build());
                await context.SaveChangesAsync();
                response = await reportingController.GetPaymentAsync(1);
            }

            public void Type_is_okObjectResult()
            {
                Assert.That(response, expression: Is.InstanceOf<OkObjectResult>());
            }

            [Test]
            public void Returns_processing_details_in_response()
            {
                var okObject = response as OkObjectResult;
                var payment = okObject.Value as Payment;
                Assert.Multiple(() =>
                {
                    Assert.That(payment.Status, Is.EqualTo("Processed"), "Status not populated with expected value");
                    Assert.That(payment.BankPaymentId, Is.EqualTo(123), "BankPaymentId not populated with expected value");
                    Assert.That(payment.Amount, Is.EqualTo(100), "Amount not populated with expected value");
                    Assert.That(payment.MaskedCardNumber, Is.EqualTo("123"), "CardNumber not populated with expected value");
                    Assert.That(payment.CurrencyISOCode, Is.EqualTo("WOW"), "CurrencyISOCode not populated with expected value");
                    Assert.That(payment.CVV, Is.EqualTo(222), "CVV not populated with expected value");
                    Assert.That(payment.ExpiryMonth, Is.EqualTo(10), "ExpiryMonth not populated with expected value");
                    Assert.That(payment.ExpiryYear, Is.EqualTo(2030), "ExpiryYear not populated with expected value");
                    Assert.That(payment.MerchantId, Is.EqualTo(Guid.Parse("10247758-5c1f-4afb-ac43-a94d2a9e5fae")), "MerchantId not populated with expected value");
                });
            }

            [Test]
            public void Logs_incoming_payment_request()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Incoming GetPaymentRequest with payment ID 1")), Is.EqualTo(1));
            }

            [Test]
            public void Logs_payment_found()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Returning payment with payment ID 1")), Is.EqualTo(1));
            }
        }

        public class GetPaymentAsync_PaymentIsNotRetrieved: ReportingControllerTests
        {
            private IActionResult response;

            [SetUp]
            public async Task SetUp()
            {
                response = await reportingController.GetPaymentAsync(1);
            }

            [Test]
            public void Type_is_notFoundObjectResult()
            {
                Assert.That(response, Is.InstanceOf<NotFoundResult>());
            }

            [Test]
            public void Logs_incoming_payment_request()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("Incoming GetPaymentRequest with payment ID 1")), Is.EqualTo(1));
            }

            [Test]
            public void Logs_payment_not_found()
            {
                Assert.That(TestCorrelator.GetLogEventsFromContextGuid(loggingContextGuid).Count(l => l.MessageTemplate.Text.Contains("No paymentId found with ID 1")), Is.EqualTo(1));
            }
        }
    }
}
