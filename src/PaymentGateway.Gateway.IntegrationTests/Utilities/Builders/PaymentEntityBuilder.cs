using PaymentGateway.Gateway.Models.Entities;
using System;

namespace PaymentGateway.Gateway.IntegrationTests.Utilities.Builders
{
    internal class PaymentEntityBuilder
    {
        public PaymentEntity Build()
        {
            return new PaymentEntity
            {
                Amount = 100,
                BankPaymentId = 123,
                CardNumber = 1231231,
                CurrencyISOCode = "WOW",
                CVV = 222,
                ExpiryMonth = 10,
                ExpiryYear = 2030,
                MerchantId = Guid.Parse("10247758-5c1f-4afb-ac43-a94d2a9e5fae"),
                Status = "Processed",
            };
        }
    }
}
