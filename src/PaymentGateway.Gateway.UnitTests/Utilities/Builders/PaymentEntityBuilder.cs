﻿using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Models.Entities;
using System;

namespace PaymentGateway.Gateway.UnitTests.Utilities.Builders
{
    internal class PaymentEntityBuilder
    {
        internal PaymentEntity Build()
        {
            return new PaymentEntity
            {
                Amount = 100,
                BankPaymentId = 123,
                EncryptedCardNumber = new byte[0],                
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
