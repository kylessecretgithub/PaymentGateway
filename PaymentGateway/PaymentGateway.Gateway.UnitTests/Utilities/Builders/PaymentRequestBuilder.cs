﻿using PaymentGateway.Gateway.Models;
using System;

namespace PaymentGateway.Gateway.UnitTests.Utilities.Builders
{
    internal class PaymentRequestBuilder
    {
        public PaymentRequest Build()
        {
            return new PaymentRequest
            {
                Amount = 100,
                CardNumber = 1231231,
                CurrencyISOCode = "WOW",
                CVV = 222,
                ExpiryMonth = 10,
                ExpiryYear = 2030,
                MerchantId = Guid.NewGuid()
            };
        }
    }
}
