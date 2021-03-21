using Newtonsoft.Json;
using PaymentGateway.Gateway.Models;
using System;

namespace PaymentGateway.Gateway.IntegrationTests.Utilities.Builders
{
    internal class PaymentRequestBuilder
    {
        private string currencyISOCode = "WOW";
        private int? cardNumber = 1231231;
        private int? CVV = 222;
        private int? amount = 100;
        private int? expiryMonth = 10;
        private int? expiryYear = 2030;
        private Guid? merchantId = Guid.Parse("10247758-5c1f-4afb-ac43-a94d2a9e5fae");

        internal PaymentRequest Build()
        {
            return new PaymentRequest
            {
                Amount = amount,
                CardNumber = cardNumber,
                CurrencyISOCode = currencyISOCode,
                CVV = CVV,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                MerchantId = merchantId
            };
        }

        internal string BuildJson()
        {
            return JsonConvert.SerializeObject(new PaymentRequest
            {
                Amount = amount,
                CardNumber = cardNumber,
                CurrencyISOCode = currencyISOCode,
                CVV = CVV,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                MerchantId = merchantId
            });
        }

        internal PaymentRequestBuilder WithCurrencyISOCode(string code)
        {
            currencyISOCode = code;
            return this;
        }

        internal PaymentRequestBuilder WithCVV(int? CVV)
        {
            this.CVV = CVV;
            return this;
        }

        internal PaymentRequestBuilder WithExpiryYear(int? expiryYear)
        {
            this.expiryYear = expiryYear;
            return this;
        }

        internal PaymentRequestBuilder WithExpiryMonth(int? expiryMonth)
        {
            this.expiryMonth = expiryMonth;
            return this;
        }

        internal PaymentRequestBuilder WithAmount(int? amount)
        {
            this.amount = amount;
            return this;
        }

        internal PaymentRequestBuilder WithMerchantId(Guid? merchantId)
        {
            this.merchantId = merchantId;
            return this;
        }
    }
}
