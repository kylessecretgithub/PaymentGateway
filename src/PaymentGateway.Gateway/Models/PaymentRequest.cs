using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Gateway.Models
{
    public class PaymentRequest
    {
        [Required]
        public int? CardNumber { get; set; }
        [Required]
        public int? CVV { get; set; }
        [Required]
        public int? ExpiryYear { get; set; }
        [Required]
        public int? ExpiryMonth { get; set; }
        [Required]
        public decimal? Amount { get; set; }
        [Required]
        public string CurrencyISOCode { get; set; }
        [Required]
        public Guid? MerchantId { get; set; }
    }
}
