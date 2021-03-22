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
                EncryptedCardNumber = new byte[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 253, 200, 83, 111, 57, 31, 173, 118, 154, 192, 90, 3, 57, 128, 71, 48 },
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
