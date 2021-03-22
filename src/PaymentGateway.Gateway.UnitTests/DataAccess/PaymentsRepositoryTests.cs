using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Models.Entities;
using PaymentGateway.Gateway.UnitTests.Utilities.Builders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.UnitTests.DataAccess
{
    internal class PaymentsRepositoryTests
    {
        protected PaymentGatewayContext context;
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
        internal class SaveChangesAsync_ChangesAreSaved : PaymentsRepositoryTests
        {
            private bool saved;

            [SetUp]
            public async Task SetUp()
            {
                saved = await paymentsRepository.SaveChangesAsync();
            }

            [Test]
            public void Returns_true()
            {
                Assert.True(saved);
            }
        }

        [TestFixture]
        internal class SaveChangesAsync_ExceptionSavingChanges : PaymentsRepositoryTests
        {
            private bool saved;
            private PaymentsRepository repo;

            [SetUp]
            public async Task SetUp()
            {
                Mock<PaymentGatewayContext> mockedContext = new Mock<PaymentGatewayContext>(optionsBuilder.Options);
                mockedContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws(new DbUpdateException());
                repo = new PaymentsRepository(mockedContext.Object);
                saved = await repo.SaveChangesAsync();
            }

            [Test]
            public void Returns_false()
            {
                Assert.False(saved);
            }
        }
    
        [TestFixture]
        internal class AddPaymentAsync_PaymentAddedToDbContext : PaymentsRepositoryTests
        {
            private PaymentEntity paymentEntity;

            [SetUp]
            public async Task SetUp()
            {
                await paymentsRepository.AddPaymentAsync(new PaymentRequestBuilder().Build(), new BankResponseBuilder().Build(), new byte[] { 10, 14 });
                await context.SaveChangesAsync();
                paymentEntity = await context.Payments.SingleAsync();
            }

            [Test]
            public void Entity_added_with_properties_populated()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(paymentEntity.Status, Is.EqualTo("Processed"), "Status not populated with expected value");
                    Assert.That(paymentEntity.BankPaymentId, Is.EqualTo(1), "BankPaymentId not populated with expected value");
                    Assert.That(paymentEntity.Amount, Is.EqualTo(100), "Amount not populated with expected value");
                    Assert.That(paymentEntity.EncryptedCardNumber, Is.EqualTo(new byte[] { 10, 14 }), "EncryptedCardNumber not populated with expected value");
                    Assert.That(paymentEntity.CurrencyISOCode, Is.EqualTo("WOW"), "CurrencyISOCode not populated with expected value");
                    Assert.That(paymentEntity.CVV, Is.EqualTo(222), "CVV not populated with expected value");
                    Assert.That(paymentEntity.ExpiryMonth, Is.EqualTo(10), "ExpiryMonth not populated with expected value");
                    Assert.That(paymentEntity.ExpiryYear, Is.EqualTo(2030), "ExpiryYear not populated with expected value");
                    Assert.That(paymentEntity.MerchantId, Is.EqualTo(Guid.Parse("10247758-5c1f-4afb-ac43-a94d2a9e5fae")), "MerchantId not populated with expected value");
                    Assert.That(paymentEntity.Created, Is.Not.Null, "Creates is not populated");
                });
            }
        }

        [TestFixture]
        internal class GetPaymentAsync_PaymentInDatabase : PaymentsRepositoryTests
        {
            private Payment payment;

            [SetUp]
            public async Task SetUp()
            {
                await context.AddAsync(new PaymentEntityBuilder().Build());
                await context.SaveChangesAsync();
                payment = await paymentsRepository.GetPaymentAsync(1);
            }

            [Test]
            public void Payment_retrieved_with_properties_populated()
            {
                var encryptedCardNumber = new byte[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 253, 200, 83, 111, 57, 31, 173, 118, 154, 192, 90, 3, 57, 128, 71, 48 };
                Assert.Multiple(() =>
                {
                    Assert.That(payment.Status, Is.EqualTo("Processed"), "Status not populated with expected value");
                    Assert.That(payment.BankPaymentId, Is.EqualTo(123), "BankPaymentId not populated with expected value");
                    Assert.That(payment.Amount, Is.EqualTo(100), "Amount not populated with expected value");
                    Assert.That(payment.EncryptedCardNumber, Is.EqualTo(encryptedCardNumber), "EncryptedCardNumber not populated with expected value");
                    Assert.That(payment.CurrencyISOCode, Is.EqualTo("WOW"), "CurrencyISOCode not populated with expected value");
                    Assert.That(payment.CVV, Is.EqualTo(222), "CVV not populated with expected value");
                    Assert.That(payment.ExpiryMonth, Is.EqualTo(10), "ExpiryMonth not populated with expected value");
                    Assert.That(payment.ExpiryYear, Is.EqualTo(2030), "ExpiryYear not populated with expected value");
                    Assert.That(payment.MerchantId, Is.EqualTo(Guid.Parse("10247758-5c1f-4afb-ac43-a94d2a9e5fae")), "MerchantId not populated with expected value");
                });
            }
        }

        [TestFixture]
        internal class GetPaymentAsync_EmptyDatabase : PaymentsRepositoryTests
        {
            private Payment payment;

            [SetUp]
            public async Task SetUp()
            {
                payment = await paymentsRepository.GetPaymentAsync(1);
            }

            [Test]
            public void Payment_is_null()
            {
                Assert.That(payment, Is.Null);
            }
        }

        [TestFixture]
        internal class GetPaymentAsync_NoMatchingEntity : PaymentsRepositoryTests
        {
            private Payment payment;

            [SetUp]
            public async Task SetUp()
            {
                await context.AddAsync(new PaymentEntityBuilder().Build());
                await context.SaveChangesAsync();
                payment = await paymentsRepository.GetPaymentAsync(10000000);
            }

            [Test]
            public void Payment_is_null()
            {
                Assert.That(payment, Is.Null);
            }
        }
    }
}
