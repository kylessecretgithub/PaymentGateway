using PaymentGateway.Gateway.Models;
using System;

namespace PaymentGateway.Gateway.UnitTests.Utilities.Builders
{
    internal class PaymentRequestBuilder
    {
        private string currencyISOCode = "WOW";
        internal PaymentRequest Build()
        {
            return new PaymentRequest
            {
                Amount = 100,
                CardNumber = 1231231,
                CurrencyISOCode = currencyISOCode,
                CVV = 222,
                ExpiryMonth = 10,
                ExpiryYear = 2030,
                MerchantId = Guid.Parse("10247758-5c1f-4afb-ac43-a94d2a9e5fae")                
            };
        }

        internal PaymentRequestBuilder WithCurrencyISOCode(string code)
        {
            currencyISOCode = code;
            return this;
        }
    }
}
