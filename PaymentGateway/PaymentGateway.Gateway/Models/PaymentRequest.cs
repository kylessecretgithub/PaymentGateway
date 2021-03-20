using System;

namespace PaymentGateway.Gateway.Models
{
    public class PaymentRequest
    {
        public int CardNumber { get; set; }
        public int CVV { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyISOCode { get; set; }
        public Guid MerchantId { get; set; }
    }
}
