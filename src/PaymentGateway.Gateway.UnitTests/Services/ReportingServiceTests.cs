using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Services;
using PaymentGateway.Gateway.UnitTests.Utilities.Builders;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.UnitTests.Services
{
    internal class ReportingServiceTests
    {
        protected PaymentGatewayContext context;
        protected ReportingService reportingService;
        protected PaymentsRepository paymentsRepository;
        protected DbContextOptionsBuilder<PaymentGatewayContext> optionsBuilder;
        private SqliteConnection connection;

        [SetUp]
        public void BaseSetUp()
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
            paymentsRepository = new PaymentsRepository(context);
            reportingService = new ReportingService(paymentsRepository);
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
            private bool success;

            [SetUp]
            public async Task SetUp()
            {
                success = await reportingService.AddPaymentAsync(new PaymentRequestBuilder().Build(), new BankResponseBuilder().Build());
            }

            [Test]
            public void Returns_true()
            {
                Assert.That(success, Is.True);
            }
        }

        [TestFixture]
        internal class AddPaymentAsync_ErrorSavingToDb : ReportingServiceTests
        {
            private bool success;

            [SetUp]
            public async Task SetUp()
            {
                var unsaveablePaymentRequest = new PaymentRequestBuilder().WithCurrencyISOCode(null).Build();
                success = await reportingService.AddPaymentAsync(unsaveablePaymentRequest, new BankResponseBuilder().Build());
            }

            [Test]
            public void Returns_false()
            {
                Assert.That(success, Is.False);
            }
        }

    }
}
