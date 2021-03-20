﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Models.Entities;
using PaymentGateway.Gateway.UnitTests.Utilities.Builders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.UnitTests.DataAccess
{
    public class PaymentsRepositoryTests
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
        public class SaveChangesAsync_ChangesAreSaved : PaymentsRepositoryTests
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
        public class SaveChangesAsync_ExceptionSavingChanges : PaymentsRepositoryTests
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
        public class AddPayment_PaymentAddedToDbContext : PaymentsRepositoryTests
        {
            private PaymentEntity paymentEntity;

            [SetUp]
            public async Task SetUp()
            {
                await paymentsRepository.AddPayment(new PaymentRequestBuilder().Build(), new BankResponseBuilder().Build());
                context.SaveChanges();
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
                    Assert.That(paymentEntity.CardNumber, Is.EqualTo(1231231), "CardNumber not populated with expected value");
                    Assert.That(paymentEntity.CurrencyISOCode, Is.EqualTo("WOW"), "CurrencyISOCode not populated with expected value");
                    Assert.That(paymentEntity.CVV, Is.EqualTo(222), "CVV not populated with expected value");
                    Assert.That(paymentEntity.ExpiryMonth, Is.EqualTo(10), "ExpiryMonth not populated with expected value");
                    Assert.That(paymentEntity.ExpiryYear, Is.EqualTo(2030), "ExpiryYear not populated with expected value");
                    Assert.That(paymentEntity.MerchantId, Is.EqualTo(Guid.Parse("10247758-5c1f-4afb-ac43-a94d2a9e5fae")), "MerchantId not populated with expected value");
                    Assert.That(paymentEntity.Created, Is.Not.Null, "Creates is not populated");
                });
            }
        }
    }
}
